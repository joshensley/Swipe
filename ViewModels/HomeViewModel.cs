using Swipe.Areas.User.Models;
using Swipe.Models;
using Swipe.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swipe.ViewModels
{
    public class HomeViewModel
    {
        public PaginatedList<ApplicationUser> PaginatedListUsers{ get; set; }
        public ApplicationUser FeaturedUser { get; set; }
        public Liked Liked { get; set; }
        public Pass Pass { get; set; }

    }
}
