namespace SmartAttendance.Models
{
    public class Classroom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; }
    }
}
