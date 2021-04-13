using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swipe.Areas.User.Models
{
    public class Pass
    {
        public Guid ID { get; set; }
        public string ApplicationUserID { get; set; }
        public string ApplicationUserPassID { get; set; }

    }
}
