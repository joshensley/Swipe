using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swipe.Areas.User.Models;
using Swipe.Areas.User.ViewModels;
using Swipe.Data;
using Swipe.Models;
using Swipe.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Swipe.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = SD.AdminEndUser + "," + SD.CustomerEndUser)]
    public class ImageController : Controller
    {

        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext _db;

        public ImageController(
            IWebHostEnvironment environment, 
            ApplicationDbContext db)
        {
            _environment = environment;
            _db = db;
        }

        public async Task<IActionResult> Index(string id, string errorMessage, string message)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUser applicationUser = await _db.ApplicationUser
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == id);


            if (errorMessage != null)
            {
                ViewData["ErrorMessage"] = errorMessage;
            }

            if (message != null)
            {
                ViewData["Message"] = message;
            }
                

            if (applicationUser == null)
            {
                return NotFound();
            }

            var auth = new FirebaseAuthProvider(new FirebaseConfig(FirebaseKeys.apiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(FirebaseKeys.AuthEmail, FirebaseKeys.AuthPassword);

            List<Tuple<Guid, string, string>> firebaseImagesUrl = new List<Tuple<Guid, string, string>>();
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

                firebaseImagesUrl.Add(new Tuple<Guid, string, string>(item.ID, task, item.ImageFirebaseTitle));
            }

            ImageViewModel imageViewModel = new ImageViewModel()
            {
                ApplicationUser = applicationUser,
                FirebaseImagesURL = firebaseImagesUrl
            };


            return View(imageViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApplicationUserID,FileUpload")] ImageViewModel imageViewModel)
        {
            FileStream fs;
            var fileUpload = imageViewModel.FileUpload;
            var userId = imageViewModel.ApplicationUserID;

            // Extension validation
            string[] permittedExtensions = { ".jpg", ".png", ".jpeg" };
            string ext = Path.GetExtension(fileUpload.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                string errorMessage = $"Files {ext}* not allowed. Only";
                for (int i = 0; i < permittedExtensions.Length; i++)
                {
                    if (permittedExtensions.Length == 1)
                    {
                        errorMessage = errorMessage + " " + permittedExtensions[i] + "*";
                    }
                    
                    if (i == (permittedExtensions.Length - 1))
                    {
                        errorMessage = errorMessage + ", and " + permittedExtensions[i] + "*";
                    }
                    else if (i == 0)
                    {
                        errorMessage = errorMessage + " " + permittedExtensions[i] + "*";
                    }
                    else
                    {
                        errorMessage = errorMessage + ", " + permittedExtensions[i] + "*";
                    }
                }
                errorMessage = errorMessage + " allowed.";

                return RedirectToAction("Index", new { id = userId, errorMessage = errorMessage });
            };
            
            string folderName = "firebaseFiles";
            string fileName = Guid.NewGuid() + "_" + fileUpload.FileName;
            string filepath = Path.Combine(_environment.WebRootPath, $"images/{folderName}", fileName);
            string fileImagePath = Path.Combine(_environment.ContentRootPath, "wwwroot/images/firebaseFiles/", fileName);

            if (ModelState.IsValid && fileUpload.Length > 1)
            {
                // Upload file to static image 
                using (var fileStream = new FileStream(fileImagePath, FileMode.Create))
                {
                    await fileUpload.CopyToAsync(fileStream);
                }

                // Upload static image to Firebase
                fs = new FileStream(filepath, FileMode.Open);
                var auth = new FirebaseAuthProvider(new FirebaseConfig(FirebaseKeys.apiKey));
                var a = await auth.SignInWithEmailAndPasswordAsync(FirebaseKeys.AuthEmail, FirebaseKeys.AuthPassword);
                var cancellation = new CancellationTokenSource();

                var upload = new FirebaseStorage(
                    FirebaseKeys.Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child("images")
                    .Child($"{fileName}")
                    .PutAsync(fs, cancellation.Token);

                try
                {
                    await upload;

                    Image image = new Image()
                    {
                        ApplicationUserID = userId,
                        ImageFirebaseTitle = fileName
                    };
                    _db.Add(image);
                    await _db.SaveChangesAsync();

                    fs.Close();
                    System.IO.File.Delete(filepath);
                    return RedirectToAction("Index", new { id = userId, message = "File Successfully Uploaded!" });

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{ex}");
                    throw;
                }
                
            }
            return BadRequest();
        }


        public async Task<IActionResult> Edit(string id, string imageTitle, string userId)
        {
            if (id == null || imageTitle == null || userId == null)
            {
                return NotFound();
            }

            var auth = new FirebaseAuthProvider(new FirebaseConfig(FirebaseKeys.apiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(FirebaseKeys.AuthEmail, FirebaseKeys.AuthPassword);

            var task = new FirebaseStorage(
                   FirebaseKeys.Bucket,
                   new FirebaseStorageOptions
                   {
                       AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                       ThrowOnCancel = true
                   })
                   .Child("images")
                   .Child($"{imageTitle}")
                   .GetDownloadUrlAsync().Result;


            Tuple<string, string, string> firebaseImagesUrl = new Tuple<string, string, string>(id, task, imageTitle);

            ImageEditViewModel imageEditViewModel = new ImageEditViewModel()
            {
                FirebaseImagesUrl = firebaseImagesUrl
            };

            return View(imageEditViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deleteImage = await _db.Images.FirstOrDefaultAsync(x => x.ID.ToString() == id);

            if (deleteImage == null)
            {
                return NotFound();
            }

            var auth = new FirebaseAuthProvider(new FirebaseConfig(FirebaseKeys.apiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(FirebaseKeys.AuthEmail, FirebaseKeys.AuthPassword);

            var task = new FirebaseStorage(
                   FirebaseKeys.Bucket,
                   new FirebaseStorageOptions
                   {
                       AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                       ThrowOnCancel = true
                   })
                   .Child("images")
                   .Child($"{deleteImage.ImageFirebaseTitle}").DeleteAsync();

            try
            {
                await task;
                _db.Remove(deleteImage);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", new { id = deleteImage.ApplicationUserID, message = "Image Deleted!" });

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex}");
                throw;
            }

        }
    }
}
