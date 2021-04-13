using Microsoft.AspNetCore.Identity;
using Swipe.Areas.User.Models;
using Swipe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swipe.ViewModels
{
    public class MessageViewModel
    {
        public ApplicationUser SentByApplicationUser { get; set; }
        public ApplicationUser SentToApplicationUser { get; set; }
        public IEnumerable<Message> Messages { get; set; }
        public Message Message { get; set; }
    }
}
