namespace Cumulative_Project1.Models
{
    public class Teacher
    {
        public int TeacherId { get; set; }
        public string? TeacherFirstName { get; set; }
        public string? TeacherLastName { get; set; }
        public string? EmployeeNumber { get; set; }
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        public List<Course> CoursesTaught { get; set; } = new List<Course>();
    }
}
