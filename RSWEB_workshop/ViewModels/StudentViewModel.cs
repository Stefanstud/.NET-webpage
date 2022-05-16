using Microsoft.AspNetCore.Mvc.Rendering;

namespace RSWEB_workshop.Models
{
    public class StudentViewModel
    {
        public List<Student>? Students { get; set; }
        public SelectList? IDs { get; set; }
        public int? studentIndex { get; set; }
    }
}
