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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public int ID { get; set; }
        public ApplicationUser Owner { get; set; }
        public ICollection<ApplicationUser> Participants { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Location { get; set; }
        public string Agenda { get; set; }
        public string Summary { get; set; }
        public DateTime Created { get; set; }
    }
}