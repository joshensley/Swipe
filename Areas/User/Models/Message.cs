using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Swipe.Areas.User.Models
{
    public class Message
    {
        public Guid ID { get; set; }
        public Guid SentByApplicationUserID { get; set; }
        public Guid SentToApplicationUserID { get; set; }

        [MaxLength(5000)]
        [Required(ErrorMessage = "Sending a message requires text")]
        [Display(Name = "Message")]
        public string SentMessage { get; set; }

        [Display(Name = "Sent")]
        [DisplayFormat(DataFormatString = "0:yyyy-MM-dd hh:mm:ss tt", ApplyFormatInEditMode = true)]
        public DateTime SentDate { get; set; }
    }
}
