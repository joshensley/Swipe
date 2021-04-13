using Firebase.Auth;
using Firebase.Storage;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Swipe.Areas.User.Models;
using Swipe.Data;
using Swipe.Models;
using Swipe.Utility;
using Swipe.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Swipe.Controllers
{
    [Authorize(Roles = SD.AdminEndUser + "," + SD.CustomerEndUser)]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            ILogger<HomeController> logger, 
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager
            )
        {
            _db = db;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? pageNumber)
        {
            var userId = (await _userManager.GetUserAsync(User)).Id;

            var userPreferences = await _db.Preferences
                .Include(x => x.Gender)
                .Include(x => x.Height)
                .Include(x => x.EthnicityPreferences)
                    .ThenInclude(x => x.Ethnicity)
                .FirstOrDefaultAsync(x => x.UserId.ToString() == userId.ToString());



            // Filters user ethnicity preferences
            IQueryable<ApplicationUser> SearchEthnicites(List<string> keywords)
            {
                var predicate = PredicateBuilder.New<ApplicationUser>();
                foreach (string keyword in keywords)
                {
                    predicate = predicate.Or(p => p.Ethnicity.Contains(keyword));
                }
                return _db.ApplicationUser.Where(predicate);
            }


            // If no ethnicities are selected, show all ethncities
            List<string> ethnicityPreference = new List<string>();
            foreach (var item in userPreferences.EthnicityPreferences)
            {
                ethnicityPreference.Add(item.Ethnicity.Type.ToString());
            }

            IQueryable<ApplicationUser> applicationUsersIQ;
            if (ethnicityPreference.Count() > 0)
            {
                applicationUsersIQ = SearchEthnicites(ethnicityPreference);
            }
            else
            {
                applicationUsersIQ = _db.ApplicationUser;
            }

            // Filters user gender preference
            switch (userPreferences.Gender.Type)
            {
                case "Male":
                    applicationUsersIQ = applicationUsersIQ.Where(x => x.Gender.Contains("Male"));
                    break;
                case "Female":
                    applicationUsersIQ = applicationUsersIQ.Where(x => x.Gender.Contains("Female"));
                    break;
                default:
                    break;
            }

            // Filters user height preference
            if (userPreferences.Height.HeightInInches > 0)
            {
                applicationUsersIQ = applicationUsersIQ.Where(x => x.Height.HeightInInches > userPreferences.Height.HeightInInches);
            }

            // Filters user age preference
            applicationUsersIQ = applicationUsersIQ.Where(x => x.Birthday >= DateTime.Now.AddYears((userPreferences.MaxAge * -1)) &&
                                                        x.Birthday <= DateTime.Now.AddYears((userPreferences.MinAge * -1)));

            // Filters passed and liked users 
            IQueryable<ApplicationUser> FilterUsers(List<string> keywords)
            {
                var predicate = PredicateBuilder.New<ApplicationUser>();
                foreach (var keyword in keywords)
                {
                    predicate = predicate.And(p => !p.Id.Contains(keyword));
                }
                return applicationUsersIQ.Where(predicate);
            }

            var applicationUserPass = await _db.Pass.Where(x => x.ApplicationUserID == userId).ToListAsync();
            var applicationUserLiked = await _db.Liked.Where(x => x.ApplicationUserID == userId).ToListAsync();
            List<string> filterUsers = new List<string>() { userId };
            foreach (var item in applicationUserPass)
            {
                filterUsers.Add(item.ApplicationUserPassID.ToString());
            }
            foreach (var item in applicationUserLiked)
            {
                filterUsers.Add(item.ApplicationUserLikedID.ToString());
            }

            applicationUsersIQ = FilterUsers(filterUsers);

            int pageSize = 3;
            PaginatedList<ApplicationUser> paginatedListUsers = await PaginatedList<ApplicationUser>.CreateAsync(
                applicationUsersIQ, 
                pageNumber ?? 1, 
                pageSize);

            var auth = new FirebaseAuthProvider(new FirebaseConfig(FirebaseKeys.apiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(FirebaseKeys.AuthEmail, FirebaseKeys.AuthPassword);

            for (int i = 0; i < paginatedListUsers.Count; i++)
            {
                if (paginatedListUsers[i].ProfileImage != "/images/avatar/default_avatar.png")
                {
                    paginatedListUsers[i].FirebaseProfileImageURL = new FirebaseStorage(
                    FirebaseKeys.Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child("images")
                    .Child($"{paginatedListUsers[i].ProfileImage}")
                    .GetDownloadUrlAsync().Result;
                }
                

            }

            // Get random user to display
            ApplicationUser featuredUser = new ApplicationUser();
            int listUsersCount = paginatedListUsers.Count();
            if (listUsersCount > 0)
            {
                Random random = new Random();
                int randomNumber = random.Next(listUsersCount);
                string randomUserId = paginatedListUsers[randomNumber].Id;

                IQueryable<ApplicationUser> featuredUserIQ = _db.ApplicationUser;
                featuredUser = await featuredUserIQ
                    .Include(x => x.Height)
                    .Include(x => x.About)
                    .FirstOrDefaultAsync(x => x.Id == randomUserId);
            }
            
            HomeViewModel homeViewModel = new HomeViewModel()
            {
                PaginatedListUsers = paginatedListUsers,
                FeaturedUser = featuredUser,
                Liked = new Liked(),
                Pass = new Pass()
            };

            return View(homeViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Like([Bind("ApplicationUserID","ApplicationUserLikedID")] Liked liked)
        {
            if (ModelState.IsValid)
            {
                bool likedExists = _db.Liked.Where(x => x.ApplicationUserID == liked.ApplicationUserLikedID && 
                    x.ApplicationUserLikedID == liked.ApplicationUserID).Any();

                if (likedExists == true)
                {
                    liked.IsMatch = true;
                    liked.IsNewMatch = true;

                    Liked applicationUserAlreadyLiked = await _db.Liked
                        .FirstOrDefaultAsync(x => x.ApplicationUserID == liked.ApplicationUserLikedID &&
                            x.ApplicationUserLikedID == liked.ApplicationUserID);

                    applicationUserAlreadyLiked.IsMatch = true;
                    applicationUserAlreadyLiked.IsNewMatch = true;

                    _db.Update(applicationUserAlreadyLiked);
                }
                else
                {
                    liked.IsMatch = false;
                    liked.IsNewMatch = false;
                }

                liked.ID = Guid.NewGuid();
                _db.Add(liked);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
                
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pass([Bind("ApplicationUserID", "ApplicationUserPassID")] Pass pass)
        {
            if (ModelState.IsValid)
            {
                pass.ID = Guid.NewGuid();
                _db.Add(pass);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
