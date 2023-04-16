namespace AppointmentScheduler.Models
{
    public class UsersModel
    {
        public int AppointId { get; set; }
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public Int64 ContactNo { get; set; }
        public string Password { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentDesc { get; set; }

    }
}
