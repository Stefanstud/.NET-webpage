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
    public class TeachersController : Controller
    {
        private readonly RSWEB_workshopContext _context;

        public TeachersController(RSWEB_workshopContext context)
        {
            _context = context;
        }

        // GET: Teachers
        [HttpPost]

        public string Index(string searchString, bool notUsed)
        {
            return "From [HttpPost]Index: filter on " + searchString;
        }
        public async Task<IActionResult> Index(string searchString, string educationLevel, string academicRank)
        {
            IQueryable<string> educationQuery = from m in _context.Teacher
                                                orderby m.Degree
                                                select m.Degree;
            IQueryable<string> academicQuery = from x in _context.Teacher
                                               orderby x.AcademicRank
                                               select x.AcademicRank;
            var teachers = from c in _context.Teacher
                           select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                teachers = teachers.Where(s => s.FirstName!.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(educationLevel))
            {
                teachers = teachers.Where(x => x.Degree == educationLevel);
            }
            if (!string.IsNullOrEmpty(academicRank))
            {
                teachers = teachers.Where(x => x.AcademicRank == academicRank);
            }
            var teacherVM = new TeacherViewModel
            {
                Degrees = new SelectList(await educationQuery.Distinct().ToListAsync()),
                Teachers = await teachers.ToListAsync(),
                Ranks = new SelectList(await academicQuery.Distinct().ToListAsync())

            };
            return View(teacherVM);
        }

        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            TeacherPicture viewmodel = new TeacherPicture
            {
                Teacher = teacher,
                ProfilePictureName = teacher.Picture
            };

            return View(viewmodel);
        }

        // GET: Teachers/Create
        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,HireDate,Degree,AcademicRank,OfficeNumber")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        // GET: Teachers/Edit/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,HireDate,Degree,AcademicRank,OfficeNumber,Picture")] Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.Id))
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
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teacher.FindAsync(id);
            _context.Teacher.Remove(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private string UploadedFile(TeacherPicture viewmodel)
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
        private bool TeacherExists(int id)
        {
            return _context.Teacher.Any(e => e.Id == id);
        }
    }
}
