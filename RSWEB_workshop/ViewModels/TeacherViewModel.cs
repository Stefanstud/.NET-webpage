using Microsoft.AspNetCore.Mvc.Rendering;

namespace RSWEB_workshop.Models
{
    public class TeacherViewModel
    {
        public List<Teacher>? Teachers { get; set; }
        public SelectList? Ranks { get; set; }
        public string? academicRank { get; set; }
        public string? SearchString { get; set; }
        public List<Teacher>? Teachers2 { get; set; }
        public SelectList? Degrees { get; set; }
        public string? educationLevel { get; set; }
    }
}
