using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Update;
using System;
using System.Threading.Tasks;

namespace ContosoUniversity.Website.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly ISchoolContext _db;

        public DepartmentController(ISchoolContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var departments = _db.Departments.Include(d => d.Administrator);
            return View(await departments.ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return HttpBadRequest();

            Department department = await _db.Departments
                .Include(d => d.Administrator)
                .FirstOrDefaultAsync(d => d.DepartmentID == id);

            if (department == null)
                return HttpNotFound();

            return View(department);
        }

        public ActionResult Create()
        {
            ViewBag.InstructorID = new SelectList(_db.Instructors, "ID", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Name", "Budget", "StartDate", "InstructorID")] Department department)
        {
            if (ModelState.IsValid)
            {
                var newDepartment = new Department
                {
                    Name = department.Name,
                    Budget = department.Budget,
                    StartDate = department.StartDate,
                    InstructorID = department.InstructorID,
                };

                _db.Departments.Add(newDepartment);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.InstructorID = new SelectList(_db.Instructors, "ID", "FullName", department.InstructorID);
            return View(department);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return HttpBadRequest();

            Department department = await _db.Departments
                .Include(d => d.Administrator)
                .FirstOrDefaultAsync(d => d.DepartmentID == id);
            if (department == null)
                return HttpNotFound();

            ViewBag.InstructorID = new SelectList(_db.Instructors, "ID", "FullName", department.InstructorID);

            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, byte[] rowVersion)
        {
            if (id == null)
                return HttpBadRequest();

            var departmentToUpdate = await _db.Departments.FirstOrDefaultAsync(d => d.DepartmentID == id);
            if (departmentToUpdate == null)
            {
                Department deletedDepartment = new Department();
                await TryUpdateModelAsync(deletedDepartment);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The department was deleted by another user.");
                ViewBag.InstructorID = new SelectList(_db.Instructors, "ID", "FullName", deletedDepartment.InstructorID);
                return View(deletedDepartment);
            }

            if (await TryUpdateModelAsync(departmentToUpdate))
            {
                try
                {
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            ViewBag.InstructorID = new SelectList(_db.Instructors, "ID", "FullName", departmentToUpdate.InstructorID);

            return View(departmentToUpdate);
        }

        public async Task<ActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
                return HttpBadRequest();

            Department department = await _db.Departments
                .Include(d => d.Administrator)
                .FirstOrDefaultAsync(d => d.DepartmentID == id);

            if (department == null)
            {
                if (concurrencyError.GetValueOrDefault())
                    return RedirectToAction("Index");
                return HttpNotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Department department)
        {
            try
            {
                _db.Entry(department).State = EntityState.Deleted;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = department.DepartmentID });
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Unable to delete. Try again, and if the problem persists contact your system administrator.");
                return View(department);
            }
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
