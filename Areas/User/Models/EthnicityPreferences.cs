using Swipe.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swipe.Areas.User.Models
{
    public class EthnicityPreferences
    {
        public Guid EthnicityID { get; set; }
        public Guid PreferencesID { get; set; }
        public Ethnicity Ethnicity { get; set; }
        public Preferences Preferences { get; set; }
    }
}
