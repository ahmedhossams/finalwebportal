namespace SmartAttendance.DTOs
{
    public class ClassroomDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public string InstructorName { get; set; }
    }

    public class CreateClassroomDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public int InstructorId { get; set; }
    }

    public class UpdateClassroomDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public int InstructorId { get; set; }
    }
}
