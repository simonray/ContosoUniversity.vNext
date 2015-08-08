using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Website.Controllers
{
    public class CourseController : Controller
    {
        private readonly ISchoolContext _db;

        public CourseController(ISchoolContext db)
        {
            _db = db;
        }

        public ActionResult Index(int? SelectedDepartment)
        {
            var departments = _db.Departments.OrderBy(q => q.Name).ToList();
            PopulateDepartmentsDropDownList(SelectedDepartment);
            int departmentID = SelectedDepartment.GetValueOrDefault();

            IQueryable<Course> courses = _db.Courses
                .Where(c => !SelectedDepartment.HasValue || c.DepartmentID == departmentID)
                .OrderBy(d => d.CourseID)
                .Include(d => d.Department);
            var sql = courses.ToString();
            return View(courses.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
                return HttpBadRequest();

            Course course = _db.Courses
                .Include(d => d.Department)
                .FirstOrDefault(c => c.CourseID == id);
            if (course == null)
                return HttpNotFound();

            return View(course);
        }

        public ActionResult Create()
        {
            PopulateDepartmentsDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("CourseID", "Title", "Credits", "DepartmentID")]Course course)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Courses.Add(course);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return HttpBadRequest();

            Course course = _db.Courses.FirstOrDefault(c => c.CourseID == id);
            if (course == null)
                return HttpNotFound();

            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPost(int? id)
        {
            if (id == null)
                return HttpBadRequest();

            var courseToUpdate = _db.Courses.First(c => c.CourseID == id);
            if (await TryUpdateModelAsync(courseToUpdate))
            {
                try
                {
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateDepartmentsDropDownList(courseToUpdate.DepartmentID);
            return View(courseToUpdate);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
                return HttpBadRequest();

            Course course = _db.Courses
                .Include(d => d.Department)
                .FirstOrDefault(c => c.CourseID == id);

            if (course == null)
                return HttpNotFound();

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = _db.Courses
                .Include(i => i.Instructors)
                .Include(e => e.Enrollments)
                .First(c => c.CourseID == id);

            foreach (var instructor in course.Instructors)
                _db.InstructorCourses.Remove(instructor);
            foreach (var enrollment in course.Enrollments)
                _db.Enrollments.Remove(enrollment);

            _db.Courses.Remove(course);

            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult UpdateCourseCredits()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdateCourseCredits(int? multiplier)
        {
            if (multiplier != null)
            {
                //TODO: Implement equivalent
                //ViewBag.RowsAffected = _db.Database.ExecuteSqlCommand("UPDATE Course SET Credits = Credits * {0}", multiplier);
            }
            return View();
        }

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in _db.Departments
                                   orderby d.Name
                                   select d;
            ViewBag.DepartmentID = new SelectList(departmentsQuery, "DepartmentID", "Name", selectedDepartment);
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
