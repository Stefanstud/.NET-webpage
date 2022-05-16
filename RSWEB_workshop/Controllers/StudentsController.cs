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
    public class StudentsController : Controller
    {
        private readonly RSWEB_workshopContext _context;

        public StudentsController(RSWEB_workshopContext context)
        {
            _context = context;
        }

        // GET: Students
        [HttpPost]

        public string Index(string searchString, bool notUsed)
        {
            return "From [HttpPost]Index: filter on " + searchString;
        }
        public async Task<IActionResult> Index(string searchString, string studentIndex)
        {
            IQueryable<int> studentIDquey = from m in _context.Student
                                            orderby m.StudentId
                                            select m.StudentId;

            var students = from c in _context.Student
                           select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.FirstName!.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(studentIndex))
            {
                students = students.Where(x => x.StudentId == Int32.Parse(studentIndex));
            }

            var studentVM = new StudentViewModel
            {
                IDs = new SelectList(await studentIDquey.Distinct().ToListAsync()),
                Students = await students.ToListAsync(),
            };
            return View(studentVM);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            StudentPicture viewmodel = new StudentPicture
            {
                Student = student,
                ProfilePictureName = student.Picture
            };

            return View(viewmodel);
        }

        // GET: Students/Create
        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,EnrollmentDate,StudentId,AcquiredCredits,CurrentSemester,EducationLevel")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,EnrollmentDate,StudentId,AcquiredCredits,CurrentSemester,EducationLevel,Picture")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
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
            return View(student);
        }

        // GET: Students/Delete/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Student.FindAsync(id);
            _context.Student.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.Id == id);
        }
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Enrolled(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .FirstOrDefaultAsync(m => m.Id == id);

            IQueryable<Student> studentQuery = _context.Enrollment.Where(x => x.CourseId == id).Select(x => x.Student);
            await _context.SaveChangesAsync();
            if (course == null)
            {
                return NotFound();
            }
            
            ViewBag.Message = course.Title;
            var studentVM = new StudentViewModel
            {
                Students = await studentQuery.ToListAsync(),
            };

            return View(studentVM);
        }
        private string UploadedFile(StudentPicture viewmodel)
        {
            string uniqueFileName = null;

            if (viewmodel.ProfilePictureFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Pictures");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewmodel.ProfilePictureFile.FileName);
                string fileNameWithPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewmodel.ProfilePictureFile.CopyTo(stream);
                }
            }
            return uniqueFileName;
        }
    }
}
