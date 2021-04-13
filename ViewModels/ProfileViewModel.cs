using Swipe.Areas.User.Models;
using Swipe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swipe.ViewModels
{
    public class ProfileViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        public Liked Liked { get; set; }
        public Pass Pass { get; set; }
        public List<string> FirebaseImagesURL { get; set; }
    }
}
