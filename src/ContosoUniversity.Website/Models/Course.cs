using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Course
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Number")]
        public int CourseID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [Required]
        [Range(0, 5)]
        public int Credits { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int DepartmentID { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        //TODO: Many-to-many (EF7?)
        //public virtual ICollection<Instructor> Instructors { get; set; }
        public virtual ICollection<InstructorCourse> Instructors { get; set; }
    }
}
