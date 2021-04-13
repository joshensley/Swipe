using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swipe.ViewModels
{
    public class InboxApplicationUser
    {
        public string Id { get; set; }
        public string CombinedIds { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public DateTime Birthday { get; set; }
        public string Ethnicity { get; set; }
        public string SentMessage { get; set; }
        public DateTime SentDate { get; set; }
        public bool NewMessage { get; set; }
        public string ProfileImage { get; set; }
        public string FirebaseProfileImageURL { get; set; }
        
    }
}
