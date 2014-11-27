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

namespace MeetingCatalogue.Controllers
{
    [Authorize]
    public class MeetingsController : Controller
    {
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
            return View();
        }

        // POST: Meetings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "From,To,Location,Title,Agenda,Summary,CreatedOn")] Meeting meeting)
        {
            meeting.Owner = CurrentUser;
            meeting.CreatedOn = DateTime.Now;
            meeting.Participants.Add(meeting.Owner);
            
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
        public ActionResult Edit([Bind(Include = "ID,From,To,Location,Title,Agenda,Summary")] Meeting meeting)
        {
            if (!meeting.CanEdit(CurrentUser))
            {
                return new HttpUnauthorizedResult();
            }

            if (ModelState.IsValid)
            {
                db.Entry(meeting).State = EntityState.Modified;
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

            // TODO: Send report

            return View(meeting);
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
