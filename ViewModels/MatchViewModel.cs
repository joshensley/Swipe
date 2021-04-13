using Swipe.Areas.User.Models;
using Swipe.Models;
using Swipe.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swipe.ViewModels
{
    public class MatchViewModel
    {
        public PaginatedList<ApplicationUser> PaginatedListUsers { get; set; }
        public IEnumerable<Liked> NewMatchedUser { get; set; }
    }
}
