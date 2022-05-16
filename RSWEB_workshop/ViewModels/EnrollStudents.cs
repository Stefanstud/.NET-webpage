using Microsoft.AspNetCore.Mvc.Rendering;
using RSWEB_workshop.Models;

namespace RSWEB_workshop.ViewModels
{
    public class EnrollStudents
    {
        public Course course { get; set; }

        public IEnumerable<int>? selectedStudents { get; set; }

        public IEnumerable<SelectListItem>? studentsEnrolledList { get; set; }

        public int? year { get; set; }
    }
}