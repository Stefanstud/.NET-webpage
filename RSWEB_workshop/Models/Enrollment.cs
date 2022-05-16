using System.ComponentModel.DataAnnotations;

namespace RSWEB_workshop.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public string? Semester { get; set; }
        public int? Year { get; set; }
        public int? Grade { get; set; }
        public string? SeminalUrl { get; set; }
        public string? ProjectUrl { get; set; }
        [Display(Name = "Exam Points")]

        public int? ExamPoint { get; set; }
        [Display(Name = "Seminal Points")]

        public int? SeminalPoints { get; set; }
        [Display(Name = "Project Points")]

        public int? ProjectPoints { get; set; }
        [Display(Name = "Additional Points")]

        public int? AdditionalPoints { get; set; }
        [Display(Name = "Finish Date")]

        public DateTime? FinishDate { get; set; }
        public Course? Course { get; set; }
        public Student? Student { get; set; }
    }
}
