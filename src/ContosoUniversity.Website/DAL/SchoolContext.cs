using System;
using System.Threading;
using System.Threading.Tasks;
using ContosoUniversity.Models;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking;

namespace ContosoUniversity.DAL
{
    public interface ISchoolContext : IDisposable
    {
        DbSet<Course> Courses { get; set; }
        DbSet<Department> Departments { get; set; }
        DbSet<Enrollment> Enrollments { get; set; }
        DbSet<Instructor> Instructors { get; set; }
        DbSet<Student> Students { get; set; }
        DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        DbSet<Person> People { get; set; }
        DbSet<InstructorCourse> InstructorCourses { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken requestAborted = default(CancellationToken));
        EntityEntry Entry(object entity);
    }

    public class SchoolContext : DbContext, ISchoolContext
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<InstructorCourse> InstructorCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}