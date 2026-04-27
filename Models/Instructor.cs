
namespace SmartAttendance.Models
{
    public class Instructor
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
