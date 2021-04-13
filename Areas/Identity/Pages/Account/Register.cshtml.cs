using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Swipe.Areas.User.Models;
using Swipe.Data;
using Swipe.Models;
using Swipe.Models.Admin;
using Swipe.Utility;

namespace Swipe.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        //private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db
            //IEmailSender emailSender
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _db = db;
            //_emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public IEnumerable<Ethnicity> Ethnicities { get; set; }

        [BindProperty]
        public IEnumerable<Height> Heights { get; set; }

        [BindProperty]
        public IEnumerable<Gender> Genders { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "First name is required.")]
            [Display(Name = "First name")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Gender is required.")]
            [Display(Name = "Gender")]
            public string Gender { get; set; }

            [Required(ErrorMessage = "Birthday is required.")]
            [Display(Name = "Birthday")]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime Birthday { get; set; }

            [Required(ErrorMessage = "Ethnicity is required.")]
            public string Ethnicity { get; set; }

            [Required(ErrorMessage = "Height is required.")]
            [Display(Name = "Height")]
            public Guid HeightID { get; set; }

            // Preferences for other users

            [Required(ErrorMessage = "Gender is required.")]
            [Display(Name = "Gender")]
            public Guid PreferencesGenderId { get; set; }

            [Required(ErrorMessage = "Height is required.")]
            [Display(Name = "Height")]
            public Guid PreferencesHeightId { get; set; }

            [Required(ErrorMessage = "Age is required.")]
            [Display(Name = "Minimum Age")]
            public int PreferencesMinAge { get; set; }

            [Required(ErrorMessage = "Age is required.")]
            [Display(Name = "Maximum Age")]
            public int PreferencesMaxAge { get; set; }



        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            Ethnicities = await _db.Ethnicity.ToListAsync();
            Genders = await _db.Gender.ToListAsync();
            Heights = await _db.Heights.ToListAsync();

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { 
                    UserName = Input.Email, 
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    Gender = Input.Gender,
                    Birthday = Input.Birthday,
                    Ethnicity = Input.Ethnicity,
                    HeightID = Input.HeightID,
                    ProfileImage = "/images/avatar/default_avatar.png"
                };


                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    if(!await _roleManager.RoleExistsAsync(SD.AdminEndUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser));
                    }

                    if (!await _roleManager.RoleExistsAsync(SD.CustomerEndUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.CustomerEndUser));
                    }

                    await _userManager.AddToRoleAsync(user, SD.CustomerEndUser);

                    _logger.LogInformation("User created a new account with password.");

                    /*var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);*/

                    /*await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");*/

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        // Add preferences
                        Preferences preferences = new Preferences
                        {
                            UserId = new Guid(user.Id),
                            MinAge = Input.PreferencesMinAge,
                            MaxAge = Input.PreferencesMaxAge,
                            HeightID = Input.PreferencesHeightId,
                            GenderID = Input.PreferencesGenderId
                        };

                        _db.Add(preferences);
                        await _db.SaveChangesAsync();


                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
