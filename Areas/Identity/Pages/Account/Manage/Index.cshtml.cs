using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Swipe.Data;
using Swipe.Models;
using Swipe.Models.Admin;
using Swipe.Utility;

namespace Swipe.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public IEnumerable<Ethnicity> Ethnicities { get; set; }

        [BindProperty]
        public IEnumerable<Height> Heights { get; set; }

        [BindProperty]
        public IEnumerable<Gender> Genders { get; set; }

        public class InputModel
        {

            public string UserName { get; set; }

            [Required(ErrorMessage = "First name is required.")]
            [Display(Name = "First name")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Gender is required.")]
            [Display(Name = "Gender")]
            public string Gender { get; set; }

            [Required(ErrorMessage = "Birthday is required.")]
            [Display(Name = "Birthday")]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            [DataType(DataType.Date)]
            public DateTime Birthday { get; set; }

            [Required(ErrorMessage = "Ethnicity is required.")]
            public string Ethnicity { get; set; }

            [Required(ErrorMessage = "Height is required.")]
            [Display(Name = "Height")]
            public Guid HeightID { get; set; }

            [Display(Name = "Avatar")]
            public string Avatar { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userManager = await _userManager.GetUserAsync(User);

            string firebaseImageURL = "";
            try
            {
                var auth = new FirebaseAuthProvider(new FirebaseConfig(FirebaseKeys.apiKey));
                var a = await auth.SignInWithEmailAndPasswordAsync(FirebaseKeys.AuthEmail, FirebaseKeys.AuthPassword);

                firebaseImageURL = new FirebaseStorage(
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
            catch (Exception)
            {
                firebaseImageURL = user.ProfileImage;
            }

            Input = new InputModel
            {
                UserName = userManager.UserName,
                FirstName = user.FirstName,
                Gender = user.Gender,
                Birthday = user.Birthday,
                Ethnicity = user.Ethnicity,
                HeightID = user.HeightID,
                Avatar = firebaseImageURL

            };
        }

        public async Task<IActionResult> OnGetAsync(string successMessage, string failMessage)
        {
            if (successMessage != "" || successMessage != null)
            {
                ViewData["SuccessMessage"] = successMessage;
            }

            if (failMessage != "" || failMessage != null)
            {
                ViewData["FailMessage"] = failMessage;
            }


            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Ethnicities = await _db.Ethnicity.ToListAsync();
            Genders = await _db.Gender.ToListAsync();
            Heights = await _db.Heights.ToListAsync();

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (await TryUpdateModelAsync<ApplicationUser>(
                user,
                "input",
                x => x.FirstName,
                x => x.Gender,
                x => x.Birthday,
                x => x.Ethnicity,
                x => x.HeightID))
            {
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
