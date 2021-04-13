using Microsoft.AspNetCore.Identity;
using Swipe.Areas.User.Models;
using Swipe.Models.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Swipe.Models
{
    public class ApplicationUser : IdentityUser
    {
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
        public Height Height { get; set; }

        public string ProfileImage { get; set; }

        public string FirebaseProfileImageURL { get; set; }

        public virtual About About { get; set; }

        public ICollection<Liked> Liked { get; set; }

        public ICollection<Image> Images { get; set; }

    }
}
