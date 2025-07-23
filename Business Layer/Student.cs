using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class Student
    {
        public Student()
        {
        }

        [Key]
        public int StudentId { get; set; }
        public required string StudentCode { get; set; }
        public required string StudentName { get; set; }
        public required string Password { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? AvatarPath { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
