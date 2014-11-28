using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MeetingCatalogue.Models
{
    public class Meeting
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public int ID { get; set; }
        public ApplicationUser Owner { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Location { get; set; }
        public string Title { get; set; }
        [DataType(DataType.MultilineText), AllowHtml]
        public string Agenda { get; set; }
        [DataType(DataType.MultilineText), AllowHtml]
        public string Summary { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual ICollection<ApplicationUser> Participants { get; private set; }

        public Meeting() : base()
        {
            this.Participants = new HashSet<ApplicationUser>();
        }

        public bool CanView(ApplicationUser user)
        {
            return Participants.Contains(user);
        }

        public bool CanEdit(ApplicationUser user)
        {
            // We could also allow admin users
            return user.Equals(Owner);
        }

        public bool CanAddSummary(ApplicationUser user)
        {
            return CanView(user);
        }

        public bool CanDelete(ApplicationUser user)
        {
            return user.Equals(Owner) && Participants.Count == 0;
        }
    }
}