using System.ComponentModel.DataAnnotations;
using RSWEB_workshop.Models;

namespace RSWEB_workshop.ViewModels
{
    public class TeacherPicture
    {
        public Teacher? Teacher { get; set; }

        [Display(Name = "Upload picture")]
        public IFormFile? ProfilePictureFile { get; set; }

        [Display(Name = "Picture")]
        public string? ProfilePictureName { get; set; }
    }
}
