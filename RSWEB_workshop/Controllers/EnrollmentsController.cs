#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RSWEB_workshop.Data;
using RSWEB_workshop.Models;
using RSWEB_workshop.ViewModels;

namespace RSWEB_workshop.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly RSWEB_workshopContext _context;

        public EnrollmentsController(RSWEB_workshopContext context)
        {
            _context = context;
        }

        // GET: Enrollments
        public async Task<IActionResult> Index()
        {
            var RSWEB_workshopContext = _context.Enrollment.Include(e => e.Course).Include(e => e.Student);
            return View(await RSWEB_workshopContext.ToListAsync());
        }

        // GET: Enrollments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: Enrollments/Create
        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Id");
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Id");
            return View();
        }

        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Create([Bind("Id,CourseId,StudentId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoint,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Id", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Id", enrollment.StudentId);
            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Id", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Id", enrollment.StudentId);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseId,StudentId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoint,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Id", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Id", enrollment.StudentId);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        [Authorize(Roles = "Admin")]


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollment.FindAsync(id);
            _context.Enrollment.Remove(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Teacher")]

        public async Task<IActionResult> Professor(int id, string teacher, int year)
        {


            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .FirstOrDefaultAsync(m => m.Id == id);

            
            var teacherModel = await _context.Teacher.FirstOrDefaultAsync(m => m.FirstName == teacher);
            ViewBag.teacher = teacher;
            ViewBag.course = course.Title;
            var enrollment = _context.Enrollment.Where(x => x.CourseId == id && (x.Course.FirstTeacherId == teacherModel.Id || x.Course.SecondTeacherId == teacherModel.Id))
            .Include(e => e.Course)
            .Include(e => e.Student);
            await _context.SaveChangesAsync();
            IQueryable<int?> yearsQuery = _context.Enrollment.OrderBy(m => m.Year).Select(m => m.Year).Distinct();
            IQueryable<Enrollment> enrollmentQuery = enrollment.AsQueryable();
            if (year != null && year != 0)
            {
                enrollmentQuery = enrollmentQuery.Where(x => x.Year == year);
            }
            else
            {
                enrollmentQuery = enrollmentQuery.Where(x => x.Year == yearsQuery.Max());
            }

            if (enrollment == null)
            {
                return NotFound();
            }

            ProfStudentViewModel viewmodel = new ProfStudentViewModel
            {
                enrollments = await enrollmentQuery.ToListAsync(),
                yearlist = new SelectList(await yearsQuery.ToListAsync())
            };

            return View(viewmodel);
        }
        [Authorize(Roles = "Admin,Teacher")]

        public async Task<IActionResult> ProfessorEdit(int? id, string teacher)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewBag.teacher = teacher;
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FirstName", enrollment.StudentId);
            return View(enrollment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]

        public async Task<IActionResult> ProfessorEdit(int id, string teacher, [Bind("Id,CourseId,StudentId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoint,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }
            string pom = teacher;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Professor", new { Id= enrollment.CourseId, Teacher = pom, Year = enrollment.Year });
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FirstName", enrollment.StudentId);
            return View(enrollment);
        }
        [Authorize(Roles = "Admin,Student")]

        public async Task<IActionResult> StudentVM(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.Id == id);

            ViewBag.student = student.FirstName;

            IQueryable<Enrollment> enrollment = _context.Enrollment.Where(x => x.StudentId == id)
            .Include(e => e.Course)
            .Include(e => e.Student);
            await _context.SaveChangesAsync();

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(await enrollment.ToListAsync());
        }

        // GET: Enrollments//5
        [Authorize(Roles = "Admin,Student")]

        public async Task<IActionResult> StudentEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = _context.Enrollment.Where(m => m.Id == id).Include(x => x.Student).Include(x => x.Course).First();
            IQueryable<Enrollment> enrollmentQuery = _context.Enrollment.AsQueryable();
            enrollmentQuery = enrollmentQuery.Where(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            StudentEditViewModel viewmodel = new StudentEditViewModel
            {
                Enrollment = await enrollmentQuery.Include(x => x.Student).Include(x => x.Course).FirstAsync(),
                SeminalUrlName = enrollment.SeminalUrl
            };
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FirstName", enrollment.StudentId);
            return View(viewmodel);
        }

        //THIS IS FOR STUDENT ROLE
        // POST: Enrollments//5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Student")]

        public async Task<IActionResult> StudentEdit(long id, StudentEditViewModel viewmodel)
        {
            if (id != viewmodel.Enrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    if (viewmodel.SeminalUrlFile != null)
                    {
                        string uniqueFileName = UploadedFile(viewmodel);
                        viewmodel.Enrollment.SeminalUrl = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.Enrollment.SeminalUrl = viewmodel.SeminalUrlName;
                    }

                    _context.Update(viewmodel.Enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(viewmodel.Enrollment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("StudentVM", new { id = viewmodel.Enrollment.StudentId });
            }

            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", viewmodel.Enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FirstName", viewmodel.Enrollment.StudentId);
            return View(viewmodel);
        }

        private string UploadedFile(StudentEditViewModel viewmodel)
        {
            string uniqueFileName = null;

            if (viewmodel.SeminalUrlFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/urls");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewmodel.SeminalUrlFile.FileName);
                string fileNameWithPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewmodel.SeminalUrlFile.CopyTo(stream);
                }
            }
            return uniqueFileName;
        }
        private bool EnrollmentExists(int id)
        {
            return _context.Enrollment.Any(e => e.Id == id);
        }
    }
}
