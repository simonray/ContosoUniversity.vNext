using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using ContosoUniversity.ViewModels;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ContosoUniversity.Website.Controllers
{
    public class InstructorController : Controller
    {
        private readonly ISchoolContext _db;

        public InstructorController(ISchoolContext db)
        {
            _db = db;
        }

        public async Task<ActionResult> Index(int? id, int? courseID)
        {
            var viewModel = new InstructorIndexData();

            var instructors = await _db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                    .ThenInclude(c => c.Course)
                        .ThenInclude(c => c.Department)
                .Include(i => i.Courses)
                    .ThenInclude(c => c.Course)
                        .ThenInclude(c => c.Enrollments)
                            .ThenInclude(e => e.Student)
                .OrderBy(i => i.LastName)
                .ToListAsync();

            viewModel.Instructors = instructors;

            if (id != null)
            {
                ViewBag.InstructorID = id.Value;
                viewModel.Courses = viewModel.Instructors.Where(
                    i => i.ID == id.Value).Single().Courses.Select(c => c.Course);
            }

            if (courseID != null)
            {
                ViewBag.CourseID = courseID.Value;
                viewModel.Enrollments = viewModel.Courses
                    .Where(x => x.CourseID == courseID)
                    .Single().Enrollments;
            }

            return View(viewModel);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);

            Instructor instructor = _db.Instructors
                .Include(i => i.OfficeAssignment)
                .FirstOrDefault(i => i.ID == id);

            if (instructor == null)
                return HttpNotFound();

            return View(instructor);
        }

        public ActionResult Create()
        {
            var instructor = new Instructor();
            instructor.Courses = new List<InstructorCourse>();
            PopulateAssignedCourseData(instructor);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("LastName", "FirstMidName", "HireDate", "OfficeAssignment")] Instructor instructor, string[] selectedCourses)
        {
            if (ModelState.IsValid)
            {
                if (selectedCourses != null)
                {
                    instructor.Courses = new List<InstructorCourse>();
                    foreach (var course in selectedCourses)
                    {
                        var courseToAdd = _db.Courses.First(c => c.CourseID == int.Parse(course));
                        var instructorCourse = new InstructorCourse { Course = courseToAdd, };
                        instructor.Courses.Add(instructorCourse);
                        _db.Entry(instructorCourse).State = Microsoft.Data.Entity.EntityState.Added;
                    }
                }
                _db.Entry(instructor.OfficeAssignment).State = Microsoft.Data.Entity.EntityState.Added;
                _db.Instructors.Add(instructor);
                await _db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);

            Instructor instructor = _db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                    .ThenInclude(c => c.Course)
                .Where(i => i.ID == id)
                .Single();

            PopulateAssignedCourseData(instructor);

            if (instructor == null)
                return HttpNotFound();

            return View(instructor);
        }

        private void PopulateAssignedCourseData(Instructor instructor)
        {
            var allCourses = _db.Courses;
            var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.Course.CourseID));
            var viewModel = new List<AssignedCourseData>();
            foreach (var course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.CourseID)
                });
            }
            ViewBag.Courses = viewModel;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("ID", "LastName", "FirstMidName", "HireDate", "OfficeAssignment")] Instructor instructor, string[] selectedCourses)
        {
            var instructorToUpdate = await _db.Instructors
               .Include(i => i.OfficeAssignment)
               .Include(i => i.Courses)
                    .ThenInclude(c => c.Course)
               .SingleOrDefaultAsync(i => i.ID == instructor.ID);

            if (instructorToUpdate == null)
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);

            if (await TryUpdateModelAsync(instructorToUpdate))
            {
                try
                {
                    if (String.IsNullOrWhiteSpace(instructor.OfficeAssignment.Location))
                        instructorToUpdate.OfficeAssignment = null;

                    UpdateInstructorCourses(selectedCourses, instructorToUpdate);

                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateAssignedCourseData(instructorToUpdate);
            return View(instructorToUpdate);
        }

        private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
        {
            if (selectedCourses == null)
            {
                instructorToUpdate.Courses = new List<InstructorCourse>();
                return;
            }

            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var instructorCourses = new HashSet<int>(instructorToUpdate.Courses.Select(c => c.Course.CourseID));

            foreach (var course in _db.Courses)
            {
                if (selectedCoursesHS.Contains(course.CourseID.ToString()))
                {
                    if (!instructorCourses.Contains(course.CourseID))
                    {
                        instructorToUpdate.Courses.Add(new InstructorCourse
                        {
                            Course = course,
                        });
                    }
                }
                else
                {
                    if (instructorCourses.Contains(course.CourseID))
                    {
                        instructorToUpdate.Courses.Remove(
                            instructorToUpdate.Courses.Where(c => c.CourseID == course.CourseID).Single());
                    }
                }
            }
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);

            Instructor instructor = _db.Instructors
                .Include(i => i.OfficeAssignment)
                .First(i => i.ID == id);
            if (instructor == null)
                return HttpNotFound();

            return View(instructor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Instructor instructor = _db.Instructors
              .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                    .ThenInclude(c => c.Course)
                        .ThenInclude(c => c.Department)

              .Where(i => i.ID == id)
              .Single();

            _db.Instructors.Remove(instructor);

            var departments = _db.Departments.Where(d => d.InstructorID == id);
            foreach (var department in departments)
                department.InstructorID = 0;

            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
