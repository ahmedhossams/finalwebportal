namespace SmartAttendance.DTOs
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class StudentDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<StudentCourseDto> EnrolledCourses { get; set; }
    }

    public class StudentCourseDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string InstructorName { get; set; }
    }

    public class CreateStudentDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
