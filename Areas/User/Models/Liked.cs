using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swipe.Areas.User.Models
{
    public class Liked
    {
        public Guid ID { get; set; }
        public string ApplicationUserID { get; set; }
        public string ApplicationUserLikedID { get; set; }
        public bool IsMatch { get; set; }
        public bool IsNewMatch { get; set; }
        public bool NewMessage { get; set; }

    }
}
