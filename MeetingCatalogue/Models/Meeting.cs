using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MeetingCatalogue.Models
{
    public class Meeting
    {
        // teszt comment
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public int ID { get; set; }
        public ApplicationUser Owner { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Location { get; set; }
        public string Agenda { get; set; }
        public string Summary { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual ICollection<ApplicationUser> Participants { get; private set; }

        public Meeting() : base()
        {
            this.Participants = new HashSet<ApplicationUser>();
        } 
    }
}