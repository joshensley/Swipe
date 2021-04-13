using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Authorization;
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
    public class ImagesController : Controller
    {
        public readonly ApplicationDbContext _db;

        public ImagesController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUser applicationUser = await _db.ApplicationUser
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            var auth = new FirebaseAuthProvider(new FirebaseConfig(FirebaseKeys.apiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(FirebaseKeys.AuthEmail, FirebaseKeys.AuthPassword);

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

            ProfileViewModel profileViewModel = new ProfileViewModel()
            {
                ApplicationUser = applicationUser,
                Liked = new Liked(),
                Pass = new Pass(),
                FirebaseImagesURL = firebaseImagesURL
            };


            return View(profileViewModel);
        }
    }
}
