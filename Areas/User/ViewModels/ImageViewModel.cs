using Microsoft.AspNetCore.Http;
using Swipe.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Swipe.Areas.User.ViewModels
{
    public class ImageViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserID { get; set; }
        public List<Tuple<Guid, string, string>> FirebaseImagesURL { get; set; }

        [Required(ErrorMessage = "Image is required")]
        public IFormFile FileUpload { get; set; }
    }
}
