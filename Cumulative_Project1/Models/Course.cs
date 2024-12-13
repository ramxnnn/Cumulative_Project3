namespace Cumulative_Project1.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public int TeacherId { get; set; } // Foreign key for the teacher
    }
}
