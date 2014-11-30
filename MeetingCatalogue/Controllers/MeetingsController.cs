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
using PagedList;
using MeetingCatalogue.Utilities;
using System.IO;
using System.Text;

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
                    var userId = User.Identity.GetUserId();
                    currentUser = db.Users.Find(userId);
                }
                return currentUser;
            }
        }

        // GET: Meetings
        public ActionResult Index(string sort, string q, string owned, int? page)
        {
            // Sorting, filtering and paging based on:
            // http://www.asp.net/mvc/overview/getting-started/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application

            var meetings = from m in db.Meetings
                           where m.Participants.Select(u => u.Id).Contains(CurrentUser.Id)
                           select m;

            ViewBag.FromSortParm = String.IsNullOrEmpty(sort) ? "from" : "";
            ViewBag.ToSortParm = sort == "to_desc" ? "to" : "to_desc";
            ViewBag.TitleSortParm = sort == "title" ? "title_desc" : "title";
            ViewBag.LocationSortParm = sort == "location" ? "location_desc" : "location";
            ViewBag.ParticipantsSortParm = sort == "participants_desc" ? "participants" : "participants";
            ViewBag.OrganizerSortParm = sort == "organizer" ? "organizer_desc" : "organizer";
            ViewBag.SortParam = sort;

            if (!String.IsNullOrEmpty(q))
            {
                meetings = meetings.Where(m => m.Title.Contains(q));
            }
            ViewBag.QParam = q;

            if (String.IsNullOrEmpty(owned))
            {
                meetings = meetings.Where(m => m.Owner.Id == CurrentUser.Id);
            }
            ViewBag.OwnedParam = owned;

            switch (sort)
            {
                case "organizer_desc":
                    meetings = meetings.OrderByDescending(m => m.Owner);
                    break;
                case "organizer":
                    meetings = meetings.OrderBy(m => m.Owner);
                    break;
                case "participants_desc":
                    meetings = meetings.OrderByDescending(m => m.Participants.Count);
                    break;
                case "participants":
                    meetings = meetings.OrderBy(m => m.Participants.Count);
                    break;
                case "location_desc":
                    meetings = meetings.OrderByDescending(m => m.Location);
                    break;
                case "location":
                    meetings = meetings.OrderBy(m => m.Location);
                    break;
                case "title_desc":
                    meetings = meetings.OrderByDescending(m => m.Title);
                    break;
                case "title":
                    meetings = meetings.OrderBy(m => m.Title);
                    break;
                case "to_desc":
                    meetings = meetings.OrderByDescending(m => m.To);
                    break;
                case "to":
                    meetings = meetings.OrderBy(m => m.To);
                    break;
                case "from":
                    meetings = meetings.OrderByDescending(m => m.From);
                    break;
                default:
                    meetings = meetings.OrderBy(m => m.From);
                    break;
            }

            ViewBag.CurrentUser = CurrentUser;

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(meetings.ToPagedList(pageNumber, pageSize));
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
            ViewBag.CurrentUser = CurrentUser;
            return View(meeting);
        }

        // GET: Meetings/Create
        public ActionResult Create()
        {
            var meeting = new Meeting()
            {
                Owner = CurrentUser,
            };
            meeting.Participants.Add(CurrentUser);
            
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
                AgendaUpdated = DateTime.Now,
                SummaryUpdated = DateTime.Now,
            };

            UpdateMeeting(meeting, newMeeting, Participants, true);
            
            if (ModelState.IsValid)
            {
                db.Meetings.Add(meeting);
                Mailer.SendEmail(meeting, ActionType.Created, Url);
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

            ViewBag.CurrentUser = CurrentUser;

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

            bool changed = UpdateMeeting(meeting, newMeeting, Participants, false);

            if (ModelState.IsValid)
            {
                //db.Entry(meeting).State = EntityState.Modified;
                db.SaveChanges();
                if (changed)
                {
                    Mailer.SendEmail(meeting, ActionType.Updated, Url);
                }
                return RedirectToAction("Index");
            }

            ViewBag.CurrentUser = CurrentUser;

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
            Mailer.SendEmail(meeting, ActionType.Deleted, Url);
            return RedirectToAction("Index");
        }

        // GET: Meetings/Report/5
        public ActionResult Report(int? id)
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

            var memoryStream = DocXTemplate.CreateWordDocument(meeting);

            // Set the position to the beginning of the stream.
            memoryStream.Seek(0, SeekOrigin.Begin);

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "meeting" + meeting.ID + ".docx");
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

        // POST: Meetings/UpdateAgenda
        [HttpPost]
        [ValidateInput(false)] 
        public ActionResult UpdateAgenda(int id, string text, long timestamp)
        {
            Meeting meeting = db.Meetings.Find(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }

            if (!meeting.CanView(CurrentUser))
            {
                return new HttpUnauthorizedResult();
            }

            bool success;
            if (meeting.AgendaUpdatedTicks == timestamp)
            {
                meeting.Agenda = Sanitizer.GetSafeHtmlFragment(text);
                meeting.AgendaUpdated = DateTime.Now;

                db.SaveChanges();

                success = true;
            }
            else
            {
                success = false;
            }

            return Json(new { success = success, timestamp = meeting.AgendaUpdatedTicks, text = meeting.Agenda });
        }

        // POST: Meetings/UpdateSummary
        [HttpPost]
        [ValidateInput(false)] 
        public ActionResult UpdateSummary(int id, string text, long timestamp)
        {
            Meeting meeting = db.Meetings.Find(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }

            if (!meeting.CanView(CurrentUser))
            {
                return new HttpUnauthorizedResult();
            }

            bool success;
            if (meeting.SummaryUpdatedTicks == timestamp)
            {
                meeting.Summary = Sanitizer.GetSafeHtmlFragment(text);
                meeting.SummaryUpdated = DateTime.Now;

                db.SaveChanges();

                success = true;
            }
            else
            {
                success = false;
            }

            return Json(new { success = success, timestamp = meeting.SummaryUpdatedTicks, text = meeting.Summary });
        }

        private bool UpdateMeeting(Meeting meeting, Meeting newMeeting, string userData, bool isNew)
        {
            // Parse user data
            var users = System.Web.Helpers.Json.Decode<ICollection<Participant>>(userData);
            if (users == null)
            {
                throw new HttpException(400, "Bad Request");
            }

            if (meeting.Agenda != newMeeting.Agenda)
            {
                meeting.AgendaUpdated = DateTime.Now;
            }
            if (meeting.Summary != newMeeting.Summary)
            {
                meeting.SummaryUpdated = DateTime.Now;
            }

            // Update participants
            var newUsers = users.Select(u => db.Users.Find(u.id));

            var participants = meeting.Participants.ToList();
            var ids = new HashSet<string>(users.Select(u => u.id));
            var keptParticipants = participants.Where(u => ids.Contains(u.Id));
            var removedParticipants = participants.Where(u => !(ids.Contains(u.Id)));
            var addedParticipants = users.Select(u => db.Users.Find(u.id)).Except(keptParticipants);

            bool changed = false;
            if (meeting.Title != newMeeting.Title ||
                meeting.From != newMeeting.From ||
                meeting.To != newMeeting.To ||
                meeting.Location != newMeeting.Location ||
                removedParticipants.Count() > 0 ||
                addedParticipants.Count() > 0)
            {
                changed = true;
            }

            // Update meeting data
            meeting.Title = newMeeting.Title;
            meeting.From = newMeeting.From;
            meeting.To = newMeeting.To;
            meeting.Location = newMeeting.Location;
            meeting.Agenda = Sanitizer.GetSafeHtmlFragment(newMeeting.Agenda);
            meeting.Summary = Sanitizer.GetSafeHtmlFragment(newMeeting.Summary);

            foreach (var user in removedParticipants)
            {
                meeting.Participants.Remove(user);
            }

            foreach (var user in addedParticipants)
            {
                meeting.Participants.Add(user);
            }

            return changed;
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
