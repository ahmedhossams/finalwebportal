namespace SmartAttendance.DTOs
{
    public class AttendanceDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public bool IsPresent { get; set; }
        public DateTime Date { get; set; }
    }

    public class CreateAttendanceDto
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public bool IsPresent { get; set; }
        public DateTime Date { get; set; }
    }

    public class UpdateAttendanceDto
    {
        public bool IsPresent { get; set; }
        public DateTime Date { get; set; }
    }
}
