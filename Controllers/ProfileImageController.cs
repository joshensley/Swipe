using Firebase.Auth;
using Firebase.Storage;
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
    public class ProfileImageController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileImageController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            ApplicationUser applicationUser = await _db.ApplicationUser
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (applicationUser == null)
            {
                return NotFound();
            }

            var auth = new FirebaseAuthProvider(new FirebaseConfig(FirebaseKeys.apiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(FirebaseKeys.AuthEmail, FirebaseKeys.AuthPassword);

            List<Tuple<Guid, string, string>> firebaseImagesUrl = new List<Tuple<Guid, string, string>>();
            foreach (var item in applicationUser.Images)
            {
                var Url = new FirebaseStorage(
                    FirebaseKeys.Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child("images")
                    .Child($"{item.ImageFirebaseTitle}")
                    .GetDownloadUrlAsync().Result;

                firebaseImagesUrl.Add(new Tuple<Guid, string, string>(item.ID, Url, item.ImageFirebaseTitle));

            }

            ProfileImageModel profileImageModel = new ProfileImageModel()
            {
                ApplicationUser = user,
                FirebaseImageURL = firebaseImagesUrl
            };


            return View(profileImageModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string selectedImage)
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);

            if (selectedImage == null || applicationUser == null)
            {
                return NotFound();
            }

            // Update ApplicationUser to new image name
            if (await TryUpdateModelAsync<ApplicationUser>(applicationUser))
            {
                applicationUser.ProfileImage = selectedImage;

                try
                {
                    await _db.SaveChangesAsync();
                }
                catch
                {
                    return RedirectToAction("/Account/Manage/Index", new { area = "Identity", failMessage = "Profile Image Not Changed. Please Try Again." });
                }
            }

            return RedirectToPage("/Account/Manage/Index", new { area = "Identity", successMessage = "Profile Image Changed!" });
            
        }
    }
}
