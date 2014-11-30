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
        [Display(Name="Organizer")]
        public ApplicationUser Owner { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? From { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? To { get; set; }
        public string Location { get; set; }
        [Required]
        public string Title { get; set; }
        [DataType(DataType.MultilineText), AllowHtml]
        public string Agenda { get; set; }
        public DateTime AgendaUpdated { get; set; }
        [NotMapped]
        public long AgendaUpdatedTicks { get { return this.AgendaUpdated.Ticks / TimeSpan.TicksPerSecond; } }
        [DataType(DataType.MultilineText), AllowHtml]
        public string Summary { get; set; }
        public DateTime SummaryUpdated { get; set; }
        [NotMapped]
        public long SummaryUpdatedTicks { get { return this.SummaryUpdated.Ticks / TimeSpan.TicksPerSecond; } }
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
            return user.Equals(Owner) && To > DateTime.Now;
        }
    }
}