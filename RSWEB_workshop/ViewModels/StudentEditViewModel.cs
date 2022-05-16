using RSWEB_workshop.Models;
using System.ComponentModel.DataAnnotations;

namespace RSWEB_workshop.ViewModels
{
    public class StudentEditViewModel
    {
        public Enrollment Enrollment { get; set; }

        [Display(Name = "Seminal File")]
        public IFormFile? SeminalUrlFile { get; set; }

        public string? SeminalUrlName { get; set; }
    }
}