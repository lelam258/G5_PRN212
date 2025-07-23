using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class Instructor
    {
        [Key]
        public int InstructorId { get; set; }
        public required string InstructorName { get; set; }
        public required int Experience { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
