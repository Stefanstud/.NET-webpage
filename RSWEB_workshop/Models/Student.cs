using System.ComponentModel.DataAnnotations;

namespace RSWEB_workshop.Models
{
    public class Student
    {
        public int Id { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Enrollment Date")]
        public DateTime? EnrollmentDate { get; set; }
        public int StudentId { get; set; }
        [Display(Name = "Credits")]
        public int? AcquiredCredits { get; set; }
        [Display(Name = "Semester")]
        public int? CurrentSemester { get; set; }
        [Display(Name = "Education Level")]
        public string? EducationLevel { get; set; }
        public ICollection<Enrollment>? CoursesEnrolled { get; set; }
        public string? Picture { get; set; }

    }
}
