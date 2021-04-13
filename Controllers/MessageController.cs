using Firebase.Auth;
using Firebase.Storage;
using Google.Apis.Admin.Directory.directory_v1.Data;
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
    public class MessageController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;

        public MessageController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string id)
        {
            // Logged in userId will be first in the split
            var ids = id.Split("~");

            var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            var userAuth = await _db.Liked.FirstOrDefaultAsync(x => x.ApplicationUserID == ids[0] && x.ApplicationUserLikedID == ids[1] && x.IsMatch == true);
            // Only the logged in user can view the page if it matches one of the ids
            if (userId == ids[0].ToString() || userId == ids[1].ToString() && userAuth.IsMatch == true)
            {
                IQueryable<Message> sentByApplicationUserIQ = _db.Messages
                .Where(x => (x.SentByApplicationUserID.ToString() == ids[0].ToString()) &&
                            (x.SentToApplicationUserID.ToString() == ids[1].ToString()));

                IQueryable<Message> sentToApplicationUserIQ = _db.Messages
                    .Where(x => (x.SentByApplicationUserID.ToString() == ids[1].ToString()) &&
                                (x.SentToApplicationUserID.ToString() == ids[0].ToString()));

                IEnumerable<Message> messages = await sentToApplicationUserIQ.Union(sentByApplicationUserIQ)
                    .OrderByDescending(x => x.SentDate)
                    .ToListAsync();

                ApplicationUser sentByApplicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(x => x.Id == ids[0].ToString());
                ApplicationUser sentToApplicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(x => x.Id == ids[1].ToString());

                var auth = new FirebaseAuthProvider(new FirebaseConfig(FirebaseKeys.apiKey));
                var a = await auth.SignInWithEmailAndPasswordAsync(FirebaseKeys.AuthEmail, FirebaseKeys.AuthPassword);

                // Get sentByApplicationUser image from Firebase 
                if (sentByApplicationUser.ProfileImage != "/images/avatar/default_avatar.png")
                {
                    sentByApplicationUser.FirebaseProfileImageURL = new FirebaseStorage(
                        FirebaseKeys.Bucket,
                        new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                            ThrowOnCancel = true
                        })
                        .Child("images")
                        .Child($"{sentByApplicationUser.ProfileImage}")
                        .GetDownloadUrlAsync().Result;
                }

                // Get sentToApplicationUser image from Firebase
                if (sentToApplicationUser.ProfileImage != "/images/avatar/default_avatar.png")
                {
                    sentToApplicationUser.FirebaseProfileImageURL = new FirebaseStorage(
                        FirebaseKeys.Bucket,
                        new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                            ThrowOnCancel = true
                        })
                        .Child("images")
                        .Child($"{sentToApplicationUser.ProfileImage}")
                        .GetDownloadUrlAsync().Result;
                }

                MessageViewModel messageViewModel = new MessageViewModel()
                {
                    SentByApplicationUser = sentByApplicationUser,
                    SentToApplicationUser = sentToApplicationUser,
                    Messages = messages,
                    Message = new Message()
                };

                var newMatch = await _db.Liked.FirstOrDefaultAsync(x => x.ApplicationUserID == ids[0].ToString() && 
                    x.ApplicationUserLikedID == ids[1].ToString());

                if (newMatch != null && (newMatch.IsNewMatch == true || newMatch.NewMessage == true))
                {
                    newMatch.IsNewMatch = false;
                    newMatch.NewMessage = false;
                    _db.Update(newMatch);
                    await _db.SaveChangesAsync();
                }

                return View(messageViewModel);
            }
            else
            {
                return NotFound();
            }

            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostMessage([Bind("SentToApplicationUserID,SentByApplicationUserID,SentMessage")] Message message)
        {
            var id = message.SentByApplicationUserID + "~" + message.SentToApplicationUserID;
            if (ModelState.IsValid)
            {
                message.ID = Guid.NewGuid();
                message.SentDate = DateTime.Now;
                _db.Add(message);


                var newMessage = await _db.Liked.FirstOrDefaultAsync(x => x.ApplicationUserID == message.SentToApplicationUserID.ToString() && 
                    x.ApplicationUserLikedID == message.SentByApplicationUserID.ToString());
                newMessage.NewMessage = true;
                _db.Update(newMessage);


                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Message", new { id = id });
            }

            return RedirectToAction("Index", "Message", new { id = id });
        }
    }
}
