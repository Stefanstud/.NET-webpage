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
    public class CoursesController : Controller
    {
        private readonly RSWEB_workshopContext _context;

        public CoursesController(RSWEB_workshopContext context)
        {
            _context = context;
        }

        // GET: Courses
        [HttpPost]
        public string Index(string searchString, bool notUsed)
        {
            return "From [HttpPost]Index: filter on " + searchString;
        }
        public async Task<IActionResult> Index(string searchString, string courseSemester, string courseProgramme)
        {
            IQueryable<string> semesterQuery = from m in _context.Course
                                               orderby m.Semester
                                               select m.Semester;
            IQueryable<string> programmeQuery = from x in _context.Course
                                                orderby x.Programme
                                                select x.Programme;
            var courses = from c in _context.Course
                          select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(s => s.Title!.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(courseSemester))
            {
                courses = courses.Where(x => x.Semester == courseSemester);
            }
            if (!string.IsNullOrEmpty(courseProgramme))
            {
                courses = courses.Where(x => x.Programme == courseProgramme);
            }
            var courseVM = new CourseViewModel
            {
                Semesters = new SelectList(await semesterQuery.Distinct().ToListAsync()),
                Courses = await courses.ToListAsync(),
                Programmes = new SelectList(await programmeQuery.Distinct().ToListAsync())
            };
            return View(courseVM);
        }
        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.Teacher1)
                .Include(c => c.Teacher2)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }
        // GET: Courses/Create
        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            ViewData["Teachers"] = new SelectList(_context.Set<Teacher>(), "Id", "FirstName");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Credits,Semester,Programme,EducationLevel,FirstTeacherId,SecondTeacherId")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var students = _context.Student.AsEnumerable();
            students = students.OrderBy(s => s.FirstName);
            var course = _context.Course.Where(m => m.Id == id).Include(x => x.StudentsEnrolled).First();
            IQueryable<Course> coursesQuery = _context.Course.AsQueryable();
            coursesQuery = coursesQuery.Where(m => m.Id == id);


            if (course == null)
            {
                return NotFound();
            }
            CreateCourse editCourse = new CreateCourse
            {
                course = await coursesQuery.Include(c => c.Teacher1).Include(c => c.Teacher2).FirstAsync(),
                studentsEnrolled = new MultiSelectList(students, "Id", "FirstName"),
                selectedStudents = course.StudentsEnrolled.Select(sa => sa.StudentId)
            };
            ViewData["Teachers"] = new SelectList(_context.Set<Teacher>(), "Id", "FirstName");
            return View(editCourse);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EnrollStudents viewmodel)
        {
            if (id != viewmodel.course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewmodel.course);
                    await _context.SaveChangesAsync();

                    var semester = viewmodel.course.Semester;

                    IEnumerable<int> selectedStudents = viewmodel.selectedStudents;
                    if (selectedStudents != null)
                    {
                        IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => !selectedStudents.Contains(s.StudentId) && s.CourseId == id);
                        _context.Enrollment.RemoveRange(toBeRemoved);

                        IEnumerable<int> existEnrollments = _context.Enrollment.Where(s => selectedStudents.Contains(s.StudentId) && s.CourseId == id).Select(s => s.StudentId);
                        IEnumerable<int> newEnrollments = selectedStudents.Where(s => !existEnrollments.Contains(s));

                        foreach (int studentId in newEnrollments)
                            _context.Enrollment.Add(new Enrollment { StudentId = studentId, CourseId = id, Semester = semester, Year = viewmodel.year });

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => s.CourseId == id);
                        _context.Enrollment.RemoveRange(toBeRemoved);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(viewmodel.course.Id))
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
            return View(viewmodel);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [Authorize(Roles = "Admin")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Course.FindAsync(id);
            _context.Course.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.Id == id);
        }
        [Authorize(Roles = "Admin,Teacher")]

        public async Task<IActionResult> Teaching(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.Id == id);

            IQueryable<Course> coursesQuery = _context.Course.Where(m => m.FirstTeacherId == id || m.SecondTeacherId == id);
            await _context.SaveChangesAsync();
            if (teacher == null)
            {
                return NotFound();
            }
            ViewBag.Message = teacher.FirstName;
            var courseVM = new CourseViewModel
            {
                Courses = await coursesQuery.ToListAsync(),
            };

            return View(courseVM);
        }
        
    }
}
