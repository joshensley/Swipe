using Swipe.Areas.User.Models;
using Swipe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swipe.ViewModels
{
    public class ProfileImageModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        public List<Tuple<Guid, string, string>> FirebaseImageURL { get; set; }
        public Image Image { get; set; }
    }
}
