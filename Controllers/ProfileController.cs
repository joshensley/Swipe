using Firebase.Auth;
using Firebase.Storage;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swipe.Areas.User.Models;
using Swipe.Data;
using Swipe.Models;
using Swipe.Utility;
using Swipe.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swipe.Controllers
{
    [Authorize(Roles = SD.AdminEndUser + "," + SD.CustomerEndUser)]
    public class ProfileController : Controller
    {
        public readonly ApplicationDbContext _db;
        public readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(
            ApplicationDbContext db, 
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = (await _userManager.GetUserAsync(User)).Id;

            ApplicationUser loggedInUser = await _db.ApplicationUser
                .Include(x => x.Liked)
                .FirstOrDefaultAsync(x => x.Id == userId);

            ApplicationUser applicationUser = await _db.ApplicationUser
                .Include(x => x.Height)
                .Include(x => x.About)
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (applicationUser == null || loggedInUser == null)
            {
                return NotFound();
            }

            var loggedInUserAreadyLikes = loggedInUser.Liked.Any(x => x.ApplicationUserLikedID == applicationUser.Id);
            ViewData["AlreadyLiked"] = "false";
            if (loggedInUserAreadyLikes == true)
            {
                ViewData["AlreadyLiked"] = "true";
            }


            var auth = new FirebaseAuthProvider(new FirebaseConfig(FirebaseKeys.apiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(FirebaseKeys.AuthEmail, FirebaseKeys.AuthPassword);

            if (applicationUser.ProfileImage != "/images/avatar/default_avatar.png")
            {
                applicationUser.FirebaseProfileImageURL = new FirebaseStorage(
                FirebaseKeys.Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                })
                .Child("images")
                .Child($"{applicationUser.ProfileImage}")
                .GetDownloadUrlAsync().Result;
            }
            

            List<string> firebaseImagesURL = new List<string>();
            foreach (var item in applicationUser.Images)
            {
                var task = new FirebaseStorage(
                    FirebaseKeys.Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child("images")
                    .Child($"{item.ImageFirebaseTitle}")
                    .GetDownloadUrlAsync().Result;

                firebaseImagesURL.Add(task);
            }

            ViewData["Liked"] = new Liked();

            ProfileViewModel profileViewModel = new ProfileViewModel()
            {
                ApplicationUser = applicationUser,
                Liked = new Liked(),
                Pass = new Pass(),
                FirebaseImagesURL = firebaseImagesURL
            };

            return View(profileViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Like([Bind("ApplicationUserID,ApplicationUserLikedID")] Liked liked)
        {
            if (ModelState.IsValid)
            {
                bool likeExists = _db.Liked.Where(x => x.ApplicationUserID == liked.ApplicationUserLikedID &&
                    x.ApplicationUserLikedID == liked.ApplicationUserID).Any();

                if (likeExists == true)
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

            ApplicationUser applicationUser = await _db.ApplicationUser
                .Include(x => x.Height)
                .Include(x => x.About)
                .FirstOrDefaultAsync(x => x.Id == liked.ApplicationUserID);

            ProfileViewModel profileViewModel = new ProfileViewModel()
            {
                ApplicationUser = applicationUser,
                Liked = liked
            };

            return View(profileViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pass([Bind("ApplicationUserID,ApplicationUserPassID")] Pass pass)
        {
            if (ModelState.IsValid)
            {
                pass.ID = Guid.NewGuid();
                _db.Add(pass);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Profile", new { id = pass.ApplicationUserPassID });
        }
    }
}
