using System;
using System.Linq;
using System.Collections.Generic;
using ContosoUniversity.Models;
using Microsoft.Data.Entity;

//TODO: Configuration/Seeding (EF7?)
namespace ContosoUniversity.DAL
{
    public class SchoolInitializer
    {
        ISchoolContext _context;

        public SchoolInitializer(ISchoolContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            if (!((DbContext)_context).Database.EnsureCreated())
                return;

            var students = new List<Student>
            {
                new Student{FirstMidName="Carson",LastName="Alexander",EnrollmentDate=new DateTime(2013, 09, 01)},
                new Student{FirstMidName="Meredith",LastName="Alonso",EnrollmentDate=new DateTime(2002, 09, 01)},
                new Student{FirstMidName="Arturo",LastName="Anand",EnrollmentDate=new DateTime(2009, 09, 01)},
                new Student{FirstMidName="Gytis",LastName="Barzdukas",EnrollmentDate=new DateTime(2002, 09, 01)},
                new Student{FirstMidName="Yan",LastName="Li",EnrollmentDate=new DateTime(2009, 09, 01)},
                new Student{FirstMidName="Peggy",LastName="Justice",EnrollmentDate=new DateTime(2015, 01, 01)},
                new Student{FirstMidName="Laura",LastName="Norman",EnrollmentDate=new DateTime(2014, 09, 01)},
                new Student{FirstMidName="Nino",LastName="Olivetto",EnrollmentDate=new DateTime(2005, 09, 01)}
            };
            students.ForEach(s => _context.Students.Add(s));
            _context.SaveChanges();

            var departments = new List<Department>
            {
                new Department{Name="Science",StartDate=DateTime.Today.AddDays(-10)},
                new Department{Name="Mathematics",StartDate=DateTime.Today.AddDays(-20)},
                new Department{Name="English",StartDate=DateTime.Today.AddDays(-30)},
            };
            departments.ForEach(s => _context.Departments.Add(s));
            _context.SaveChanges();

            var courses = new List<Course>
            {
                new Course{CourseID=1050,Title="Chemistry",Credits=3,DepartmentID=1},
                new Course{CourseID=4022,Title="Microeconomics",Credits=3,DepartmentID=2},
                new Course{CourseID=4041,Title="Macroeconomics",Credits=3,DepartmentID=2},
                new Course{CourseID=1045,Title="Calculus",Credits=4,DepartmentID=2},
                new Course{CourseID=3141,Title="Trigonometry",Credits=4,DepartmentID=2},
                new Course{CourseID=2021,Title="Composition",Credits=3,DepartmentID=2},
                new Course{CourseID=2042,Title="Literature",Credits=4,DepartmentID=3}
            };
            courses.ForEach(s => _context.Courses.Add(s));
            _context.SaveChanges();

            var instructor = new Instructor { LastName = "Smith", FirstMidName = "John", HireDate = DateTime.Today.AddDays(-30) };
            _context.Instructors.Add(instructor);
            instructor.Courses = new List<InstructorCourse>() {
                new InstructorCourse { Course = _context.Courses.Where(o => o.CourseID == 1050).Single() },
                new InstructorCourse { Course = _context.Courses.Where(o => o.CourseID == 4022).Single() },
                new InstructorCourse { Course = _context.Courses.Where(o => o.CourseID == 4041).Single() },
            };
            instructor.OfficeAssignment = new OfficeAssignment { Location = "London" };

            instructor = new Instructor { LastName = "Glare", FirstMidName = "Tony", HireDate = DateTime.Today.AddDays(-50) };
            _context.Instructors.Add(instructor);
            instructor.Courses = new List<InstructorCourse>() {
                        new InstructorCourse { Course = _context.Courses.Where(o => o.CourseID == 1045).Single() },
                        new InstructorCourse { Course = _context.Courses.Where(o => o.CourseID == 3141).Single() },
                        new InstructorCourse { Course = _context.Courses.Where(o => o.CourseID == 2021).Single() },
                        new InstructorCourse { Course = _context.Courses.Where(o => o.CourseID == 2042).Single() },
            };
            instructor.OfficeAssignment = new OfficeAssignment { Location = "London" };
            _context.SaveChanges();

            var inst1 = _context.Instructors.Where(x => x.LastName == "Smith").Single();
            var inst2 = _context.Instructors.Where(x => x.LastName == "Glare").Single();
            foreach (var department in _context.Departments)
            {
                if (department.Name == "Science" || department.Name == "English")
                    department.Administrator = inst1;
                else
                    department.Administrator = inst2;
            }
            _context.SaveChanges();

            var enrollments = new List<Enrollment>
            {
                new Enrollment{StudentID=1,CourseID=1050,Grade=Grade.A},
                new Enrollment{StudentID=1,CourseID=4022,Grade=Grade.C},
                new Enrollment{StudentID=1,CourseID=4041,Grade=Grade.B},
                new Enrollment{StudentID=2,CourseID=1045,Grade=Grade.B},
                new Enrollment{StudentID=2,CourseID=3141,Grade=Grade.F},
                new Enrollment{StudentID=2,CourseID=2021,Grade=Grade.F},
                new Enrollment{StudentID=3,CourseID=1050},
                new Enrollment{StudentID=4,CourseID=1050,},
                new Enrollment{StudentID=4,CourseID=4022,Grade=Grade.F},
                new Enrollment{StudentID=5,CourseID=4041,Grade=Grade.C},
                new Enrollment{StudentID=6,CourseID=1045},
                new Enrollment{StudentID=7,CourseID=3141,Grade=Grade.A},
            };
            enrollments.ForEach(s => _context.Enrollments.Add(s));
            _context.SaveChanges();
        }
    }
}
