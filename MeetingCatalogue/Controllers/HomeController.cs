using MeetingCatalogue.DAL;
using MeetingCatalogue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MeetingCatalogue.ViewModels;

namespace MeetingCatalogue.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
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

        public ActionResult Index()
        {
            var viewModel = new DashboardViewModel();

            viewModel.Upcoming = (from meeting in db.Meetings
                                  where meeting.Participants.Select(u => u.Id).Contains(CurrentUser.Id) &&
                                      meeting.To > DateTime.Now
                                  orderby meeting.From ascending
                                  select meeting).Take(5).ToList();

            viewModel.Recent = (from meeting in db.Meetings
                                where meeting.Participants.Select(u => u.Id).Contains(CurrentUser.Id) &&
                                    meeting.From < DateTime.Now
                                orderby meeting.From descending
                                select meeting).Take(5).ToList();

            viewModel.Owned = (from meeting in db.Meetings
                               where meeting.Owner.Id == CurrentUser.Id
                               orderby meeting.From descending
                               select meeting).Take(5).ToList();

            return View(viewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}