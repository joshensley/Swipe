using Swipe.Models.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Swipe.Areas.User.Models
{
    public class Preferences
    {
        public Guid ID { get; set; }
        public Guid UserId { get; set; }

        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
        [Display(Name = "Min Age")]
        public int MinAge { get; set; }

        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
        [Display(Name = "Max Age")]
        public int MaxAge { get; set; }

        [Required(ErrorMessage = "Height is required.")]
        [Display(Name = "Height")]
        public Guid HeightID { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [Display(Name = "Gender")]
        public Guid GenderID { get; set; }

        public Gender Gender { get; set; }

        public Height Height { get; set; }

        public bool SavedPreferences { get; set; }

        public ICollection<EthnicityPreferences> EthnicityPreferences { get; set; }
    }
}
