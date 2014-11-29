using MeetingCatalogue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeetingCatalogue.ViewModels
{
    public class DashboardViewModel
    {
        public IEnumerable<Meeting> Upcoming { get; set; }
        public IEnumerable<Meeting> Recent { get; set; }
        public IEnumerable<Meeting> Owned { get; set; }
    }
}