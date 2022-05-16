using System.ComponentModel.DataAnnotations;

namespace RSWEB_workshop.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
        public string? Semester { get; set; }
        public string? Programme { get; set; }

        [Display(Name = "Education Level")]
        public string? EducationLevel { get; set; }

        [Display(Name = "First Teacher")]
        public int? FirstTeacherId { get; set; }

        public Teacher? Teacher1 { get; set; }

        [Display(Name = "Second Teacher")]
        public int? SecondTeacherId { get; set; }

        public Teacher? Teacher2 { get; set; }
        public ICollection<Enrollment>? StudentsEnrolled { get; set; }
    }
}
