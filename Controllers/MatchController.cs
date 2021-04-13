using Firebase.Auth;
using Firebase.Storage;
using LinqKit;
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
    public class MatchController : Controller
    {

        private readonly ApplicationDbContext _db;

        public MatchController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string id, int? pageNumber)
        {

            IQueryable<ApplicationUser> GetMatchedUsers(List<string> keywords)
            {
                var predicate = PredicateBuilder.New<ApplicationUser>();

                foreach (var keyword in keywords)
                {
                    predicate = predicate.Or(p => p.Id.Contains(keyword));
                }

                return _db.ApplicationUser.Where(predicate);
            }

            var applicationUserMatches = await _db.Liked
                .Where(x => x.ApplicationUserID == id && x.IsMatch == true)
                .ToListAsync();

            List<string> matches = new List<string>();
            foreach (var item in applicationUserMatches)
            {
                matches.Add(item.ApplicationUserLikedID.ToString());
            }
            
            IQueryable<ApplicationUser> applicationUsersIQ;
            if(matches.Count() > 0)
            {
                applicationUsersIQ = GetMatchedUsers(matches);
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


            IEnumerable<Liked> newMatchedUsers = await _db.Liked
                .Where(x => x.ApplicationUserID == id && x.IsMatch == true && x.IsNewMatch == true)
                .ToListAsync();

            MatchViewModel matchViewModel = new MatchViewModel()
            {
                PaginatedListUsers = paginatedListUsers,
                NewMatchedUser = newMatchedUsers
            };

            return View(matchViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlike(string id)
        {
            var ids = id.Split("~");
            string applicationUserId = ids[0].ToString();
            string otherUserId = ids[1].ToString();

            var unlike = await _db.Liked.FirstOrDefaultAsync(x => x.ApplicationUserLikedID == otherUserId && x.ApplicationUserID == applicationUserId);
            _db.Liked.Remove(unlike);


            bool likeExists = _db.Liked.Where(x => x.ApplicationUserLikedID == applicationUserId &&
                x.ApplicationUserID == otherUserId).Any();

            if (likeExists == true)
            {
                Liked otherUnlike = await _db.Liked.FirstOrDefaultAsync(x => x.ApplicationUserLikedID == applicationUserId && x.ApplicationUserID == otherUserId);
                otherUnlike.IsMatch = false;
                otherUnlike.IsNewMatch = false;
                _db.Update(otherUnlike);
            }
           
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Match", new { id = applicationUserId });
        }
    }    
}
