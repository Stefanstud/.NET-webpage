using Microsoft.AspNetCore.Mvc.Rendering;
using RSWEB_workshop.Models;

namespace RSWEB_workshop.ViewModels
{
    public class CourseViewModel
    {
        public IList<Course> Courses { get; set; }
        public SelectList Semesters { get; set; }
        public SelectList Programmes { get; set; }
        public string CourseSemester { get; set; }
        public string CourseProgramme { get; set; }
        public string SearchString { get; set; }
    }
}
