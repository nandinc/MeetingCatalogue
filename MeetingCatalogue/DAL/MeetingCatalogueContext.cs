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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Meeting>()
                .HasMany(m => m.Participants)
                .WithMany(u => u.Meetings);
        }

        public static MeetingCatalogueContext Create()
        {
            return new MeetingCatalogueContext();
        }
    }
}