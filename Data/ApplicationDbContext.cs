using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Swipe.Areas.User.Models;
using Swipe.Models;
using Swipe.Models.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swipe.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Default //
        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        // Admin //
        public DbSet<Height> Heights { get; set; }
        public DbSet<Gender> Gender { get; set; }
        public DbSet<Ethnicity> Ethnicity { get; set; }

        // User //
        public DbSet<Preferences> Preferences { get; set; }
        public DbSet<EthnicityPreferences> EthnicityPreferences { get; set; }
        public DbSet<About> About { get; set; }
        public DbSet<Liked> Liked { get; set; }
        public DbSet<Pass> Pass { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EthnicityPreferences>()
                .HasKey(x => new { x.EthnicityID, x.PreferencesID });

            
        }

        

    }
}
