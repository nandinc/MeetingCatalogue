using MeetingCatalogue.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MeetingCatalogue.DAL
{
    public class MeetingCatalogueContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Meeting> Meetings { get; set; }

        public MeetingCatalogueContext()
            : base("DefaultConnection")
        {
        }
    }
}