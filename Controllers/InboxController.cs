using Firebase.Auth;
using Firebase.Storage;
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
    public class InboxController : Controller
    {
        public readonly ApplicationDbContext _db;

        public InboxController(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<InboxApplicationUser> inboxApplicationUsers { get; set; }

        public async Task<IActionResult> Index(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _db.Liked
                .Where(x => x.ApplicationUserID == id && x.IsMatch == true)
                .ToListAsync();

            // Get list of matched users
            List<string> matchedUserIds = new List<string>();
            foreach (var item in applicationUser)
            {
                matchedUserIds.Add(item.ApplicationUserLikedID);
            }

            inboxApplicationUsers = new List<InboxApplicationUser>();

            foreach (var matchedUserId in matchedUserIds)
            {
                var user = await _db.ApplicationUser.FirstOrDefaultAsync(x => x.Id == matchedUserId);

                var auth = new FirebaseAuthProvider(new FirebaseConfig(FirebaseKeys.apiKey));
                var a = await auth.SignInWithEmailAndPasswordAsync(FirebaseKeys.AuthEmail, FirebaseKeys.AuthPassword);

                if (user.ProfileImage != "/images/avatar/default_avatar.png")
                {
                    user.FirebaseProfileImageURL = new FirebaseStorage(
                        FirebaseKeys.Bucket,
                        new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                            ThrowOnCancel = true
                        })
                        .Child("images")
                        .Child($"{user.ProfileImage}")
                        .GetDownloadUrlAsync().Result;
                }

                // Get the latest message
                var message = await _db.Messages
                    .OrderByDescending(x => x.SentDate)
                    .FirstOrDefaultAsync(
                        x => x.SentByApplicationUserID.ToString() == matchedUserId && 
                        x.SentToApplicationUserID.ToString() == id);

                var liked = await _db.Liked.FirstOrDefaultAsync(
                    x => x.ApplicationUserID == id && 
                    x.ApplicationUserLikedID == matchedUserId);

                InboxApplicationUser inboxApplicationUser = new InboxApplicationUser()
                {
                    Id = user.Id,
                    CombinedIds = id.ToString() + "~" + user.Id.ToString(),
                    FirstName = user.FirstName,
                    Gender = user.Gender,
                    Birthday = user.Birthday,
                    Ethnicity = user.Ethnicity,
                    SentMessage = message.SentMessage,
                    SentDate = message.SentDate,
                    NewMessage = liked.NewMessage,
                    ProfileImage = user.ProfileImage,
                    FirebaseProfileImageURL = user.FirebaseProfileImageURL
                };

                inboxApplicationUsers.Add(inboxApplicationUser);
            }

            inboxApplicationUsers = inboxApplicationUsers
                .OrderBy(x => x.NewMessage ? 0 : 1)
                .ToList();

            InboxViewModel inboxViewModel = new InboxViewModel()
            {
                InboxApplicationUsers = inboxApplicationUsers
            };

            return View(inboxViewModel);
        }
    }
}
