using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Swipe.Areas.User.Models;
using Swipe.Models;
using Swipe.Models.Admin;
using Swipe.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swipe.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            if (_db.Roles.Any(r => r.Name == SD.AdminEndUser)) return;

            _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.CustomerEndUser)).GetAwaiter().GetResult();

            // Seed Gender, Ethnicity, Height

            var gender = new Gender { ID = Guid.NewGuid(), Type = "No Preference" };
            var ethnicity = new Ethnicity { ID = Guid.NewGuid(), Type = "No Preference" };
            var height = new Height { ID = Guid.NewGuid(), DisplayHeight = "No Preference", HeightInInches = 0 };
            _db.Ethnicity.Add(ethnicity);
            _db.Gender.Add(gender);
            _db.Heights.Add(height);
            await _db.SaveChangesAsync();

            ApplicationUser newApplicationUser = new ApplicationUser
            {
                UserName = "joshensley@gmail.com",
                Email = "joshensley@gmail.com",
                FirstName = "Admin",
                EmailConfirmed = true,
                Gender = gender.Type,
                Birthday = DateTime.Now,
                Ethnicity = ethnicity.Type,
                HeightID = height.ID,
                ProfileImage = "/images/avatar/default_avatar.png",
                
            };

            _userManager.CreateAsync(newApplicationUser, "Teton34Otter!").GetAwaiter().GetResult();
            //var result = await _userManager.CreateAsync(newApplicationUser, "Teton34Otter!");

            ApplicationUser user = await _db.Users.FirstOrDefaultAsync(u => u.Email == "joshensley@gmail.com");

            // Seed Preferences
            Preferences preferences = new Preferences
            {
                UserId = new Guid(user.Id),
                MinAge = 18,
                MaxAge = 100,
                HeightID = height.ID,
                GenderID = gender.ID
            };
            _db.Add(preferences);
            await _db.SaveChangesAsync();

            _userManager.AddToRoleAsync(user, SD.AdminEndUser).GetAwaiter().GetResult();
        }
    }
}
