using Swipe.Models;
using Swipe.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swipe.ViewModels
{
    public class LikedViewModel
    {
        public PaginatedList<ApplicationUser> PaginatedListUsers { get; set; }
    }
}
