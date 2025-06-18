using System;

namespace WpfApp1.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }

    public enum UserRole
    {
        Administrator = 1,
        Doctor = 2,
        Nurse = 3,
        Receptionist = 4
    }
}
