using Microsoft.Build.Framework;
using System.Numerics;

namespace AppointmentScheduler.Models
{
    public class LoginModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public Int64 ContactNo { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Cmpy_name { get; set; }
        
        public string message { get; set; } 

    }
}
