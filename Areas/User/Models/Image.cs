using Swipe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swipe.Areas.User.Models
{
    public class Image
    {
        public Guid ID { get; set; }
        public string ApplicationUserID { get; set; }
        public string ImageFirebaseTitle { get; set; }
    }
}
