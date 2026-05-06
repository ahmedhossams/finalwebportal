namespace SmartAttendance.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string InstructorName { get; set; }
    }

    public class CourseDetailDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int InstructorId { get; set; }
        public string InstructorName { get; set; }
        public int EnrolledStudentsCount { get; set; }
    }

    public class CreateCourseDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int InstructorId { get; set; }
    }

    public class UpdateCourseDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int InstructorId { get; set; }
    }
}
