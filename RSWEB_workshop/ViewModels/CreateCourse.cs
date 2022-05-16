using Microsoft.AspNetCore.Mvc.Rendering;
using RSWEB_workshop.Models;

namespace RSWEB_workshop.ViewModels
{
    public class CreateCourse
    {
        public Course? course { get; set; }

        public IEnumerable<int>? selectedStudents { get; set; }

        public IEnumerable<SelectListItem>? studentsEnrolled { get; set; }
    

    }
}