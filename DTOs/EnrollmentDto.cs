namespace SmartAttendance.DTOs
{
    public class EnrollmentDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
    }

    public class CreateEnrollmentDto
    {
        public int CourseId { get; set; }
    }
}
