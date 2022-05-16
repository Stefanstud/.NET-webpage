using Microsoft.AspNetCore.Mvc.Rendering;
using RSWEB_workshop.Models;

namespace RSWEB_workshop.ViewModels
{
    public class ProfStudentViewModel
    {
        public List<Enrollment>? enrollments { get; set; }
        public SelectList yearlist { get; set; }
        public int? year { get; set; }
    }
}
