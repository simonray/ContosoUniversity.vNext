using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class InstructorCourse
    {
        public int ID { get; set; }

        [ForeignKey("Course")]
        public int CourseID { get; set; }
        public Course Course { get; set; }
    }
}
