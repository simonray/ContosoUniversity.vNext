using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using ContosoUniversity.ViewModels;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Framework.OptionsModel;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ContosoUniversity.Website.Controllers
{
    public class StudentController : Controller
    {
        private readonly ISchoolContext _db;
        private IOptions<AppSettings> _settings { get; set; }

        public StudentController(ISchoolContext db, IOptions<AppSettings> settings)
        {
            _db = db;
            _settings = settings;
        }

        public ActionResult Index(string sortOrder, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.SearchString = searchString;

            var students = from s in _db.Students select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students
                    .Where(s => 
                    s.LastName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase) ||
                    s.FirstMidName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            var index = page ?? 1;
            var total = students.Count();            
            var size = _settings.Options.PageSize;

            var model = students
                .Skip((index-1) * size)
                .Take(size)
                .ToList();

            return View(new PagedList<Student>(index, size, total, model));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);

            Student student = _db.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .FirstOrDefault(s => s.ID == id);

            if (student == null)
                return HttpNotFound();

            return View(student);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("LastName", "FirstMidName", "EnrollmentDate")] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Students.Add(student);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(student);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);

            Student student = _db.Students.FirstOrDefault(s => s.ID == id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPost(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);

            var studentToUpdate = _db.Students.FirstOrDefault(s => s.ID == id);
            if (await TryUpdateModelAsync(studentToUpdate))
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
            return View(studentToUpdate);
        }

        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);

            if (saveChangesError.GetValueOrDefault())
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";

            Student student = _db.Students.FirstOrDefault(s => s.ID == id);
            if (student == null)
                return HttpNotFound();

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Student student = _db.Students.First(s => s.ID == id);
                _db.Students.Remove(student);
                _db.SaveChanges();
            }
            catch (Exception)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }

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
