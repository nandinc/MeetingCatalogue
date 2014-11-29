using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MeetingCatalogue.DAL;
using MeetingCatalogue.Models;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Security.Application;

namespace MeetingCatalogue.Controllers
{
    [Authorize]
    public class MeetingsController : Controller
    {
        private class Participant
        {
            public string id { get; set; }
            public string text { get; set; }
            public bool locked { get; set; }
        }
        
        private MeetingCatalogueContext db = new MeetingCatalogueContext();

        private ApplicationUser currentUser;
        private ApplicationUser CurrentUser
        {
            get
            {
                if (currentUser == null)
                {
                    //var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    var userId = User.Identity.GetUserId();
                    currentUser = db.Users.Find(userId);
                }
                return currentUser;
            }
        }

        // GET: Meetings
        public ActionResult Index()
        {
            return View(db.Meetings.ToList());
        }

        // GET: Meetings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meeting meeting = db.Meetings.Find(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }
            if (!meeting.CanView(CurrentUser))
            {
                return new HttpUnauthorizedResult();
            }
            return View(meeting);
        }

        // GET: Meetings/Create
        public ActionResult Create()
        {
            var meeting = new Meeting()
            {
                Owner = CurrentUser,
                CreatedOn = DateTime.Now,
            };
            meeting.Participants.Add(meeting.Owner);
            
            return View(meeting);
        }

        // POST: Meetings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "From,To,Location,Title,Agenda,Summary")] Meeting newMeeting, string Participants)
        {
            var meeting = new Meeting()
            {
                Owner = CurrentUser,
                CreatedOn = DateTime.Now,
            };

            updateMeeting(meeting, newMeeting, Participants, true);
            
            if (ModelState.IsValid)
            {
                db.Meetings.Add(meeting);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(meeting);
        }

        // GET: Meetings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meeting meeting = db.Meetings.Find(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }

            if (!meeting.CanEdit(CurrentUser))
            {
                return new HttpUnauthorizedResult();
            }

            return View(meeting);
        }

        // POST: Meetings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,From,To,Location,Title,Agenda,Summary")] Meeting newMeeting, string Participants)
        {
            var meeting = db.Meetings.Find(newMeeting.ID);
            if (meeting == null)
            {
                return HttpNotFound();
            }

            if (!meeting.CanEdit(CurrentUser))
            {
                return new HttpUnauthorizedResult();
            }

            updateMeeting(meeting, newMeeting, Participants, false);

            if (ModelState.IsValid)
            {
                //db.Entry(meeting).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(meeting);
        }

        // GET: Meetings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meeting meeting = db.Meetings.Find(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }
            if (!meeting.CanDelete(CurrentUser))
            {
                return new HttpUnauthorizedResult();
            }
            return View(meeting);
        }

        // POST: Meetings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Meeting meeting = db.Meetings.Find(id);
            if (!meeting.CanDelete(CurrentUser))
            {
                return new HttpUnauthorizedResult();
            }
            db.Meetings.Remove(meeting);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Meetings/GenerateReport/5
        public ActionResult GenerateReport(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meeting meeting = db.Meetings.Find(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }

            if (!meeting.CanView(CurrentUser))
            {
                return new HttpUnauthorizedResult();
            }

            return View(meeting);
        }

        // POST: Meetings/GenerateReport/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateReportConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meeting meeting = db.Meetings.Find(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }

            if (!meeting.CanView(CurrentUser))
            {
                return new HttpUnauthorizedResult();
            }

            // TODO: Send report

            return View(meeting);
        }

        // POST: Meetings/SearchParticipants?q=Username
        [HttpPost]
        public ActionResult SearchParticipants(string q)
        {
            var users = from user in db.Users
                        where user.UserName.StartsWith(q) || user.Email.StartsWith(q)
                        orderby user.UserName
                        select new
                        {
                            id = user.Id,
                            text = user.UserName,
                        };

            return Json(users);
        }

        private void updateMeeting(Meeting meeting, Meeting newMeeting, string userData, bool isNew)
        {
            // Parse user data
            var users = System.Web.Helpers.Json.Decode<ICollection<Participant>>(userData);
            if (users == null)
            {
                throw new HttpException(400, "Bad Request");
            }

            // Update meeting data
            meeting.Title = newMeeting.Title;
            meeting.From = newMeeting.From;
            meeting.To = newMeeting.To;
            meeting.Location = newMeeting.Location;
            meeting.Agenda = Sanitizer.GetSafeHtmlFragment(newMeeting.Agenda);
            meeting.Summary = Sanitizer.GetSafeHtmlFragment(newMeeting.Summary);

            // Update participants
            var newUsers = users.Select(u => db.Users.Find(u.id));

            var participants = meeting.Participants.ToList();
            var ids = new HashSet<string>(users.Select(u => u.id));
            var keptParticipants = participants.Where(u => ids.Contains(u.Id));
            var removedParticipants = participants.Where(u => !(ids.Contains(u.Id)));
            var addedParticipants = users.Select(u => db.Users.Find(u.id)).Except(keptParticipants);

            foreach (var user in removedParticipants)
            {
                meeting.Participants.Remove(user);
            }

            foreach (var user in addedParticipants)
            {
                meeting.Participants.Add(user);
            }

            // Send e-mail, etc.
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
