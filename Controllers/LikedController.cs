using Firebase.Auth;
using Firebase.Storage;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class LikedController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LikedController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string id, int? pageNumber)
        {
            // Filters the people the current logged in user likes
            IQueryable<ApplicationUser> GetLikedUsers(List<string> keywords)
            {
                var predicate = PredicateBuilder.New<ApplicationUser>();

                foreach (string keyword in keywords)
                {
                    predicate = predicate.Or(p => p.Id.Contains(keyword));
                }

                return _db.ApplicationUser.Where(predicate);
            }

            var likedUsers = await _db.Liked
                .Where(x => x.ApplicationUserID == id)
                .ToListAsync();

            List<string> likedUserList = new List<string>();
            foreach (var item in likedUsers)
            {
                likedUserList.Add(item.ApplicationUserLikedID.ToString());
            }

            IQueryable<ApplicationUser> applicationUsersIQ;
            if (likedUserList.Count() > 0)
            {
                applicationUsersIQ = GetLikedUsers(likedUserList);
            }
            else
            {
                applicationUsersIQ = _db.ApplicationUser.Where(x => x.Id == "");
            }

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


            LikedViewModel likedViewModel = new LikedViewModel()
            {
                PaginatedListUsers = paginatedListUsers
            };

            return View(likedViewModel);
        }
    }
}
