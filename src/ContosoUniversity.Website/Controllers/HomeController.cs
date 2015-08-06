using ContosoUniversity.DAL;
using ContosoUniversity.ViewModels;
using Microsoft.AspNet.Mvc;
using System.Linq;

namespace ContosoUniversity.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISchoolContext _db;

        public HomeController(ISchoolContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }

        public ActionResult About()
        {
            IQueryable<EnrollmentDateGroup> data =
                from student in _db.Students
                    orderby student.EnrollmentDate descending
                    group student by student.EnrollmentDate into dateGroup
                    select new EnrollmentDateGroup()
                    {
                        EnrollmentDate = dateGroup.Key,
                        StudentCount = dateGroup.Count()
                    };
            return View(data.ToList());
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
