using System.ComponentModel.DataAnnotations;

namespace RSWEB_workshop.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Hire Date")]
        public DateTime? HireDate { get; set; }
        public string? Degree { get; set; }
        [Display(Name = "Academic Rank")]
        public string? AcademicRank { get; set; }
        [Display(Name = "Office Number")]
        public string? OfficeNumber { get; set; }
        [Display(Name = "Assigned Courses:")]
        public ICollection<Course>? Courses1 { get; set; }
        public ICollection<Course>? Courses2 { get; set; } 
        public string? Picture { get; set; }


    }
}
