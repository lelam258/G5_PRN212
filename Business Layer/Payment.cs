using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        [ForeignKey("Student")]
        public required int StudentId { get; set; }
        [ForeignKey("LifeSkillCourse")]
        public required int CourseId { get; set; }
        public required decimal Amount { get; set; }
        public required DateTime PaymentDate { get; set; }
        public required string Status { get; set; } 
        public Student? Student { get; set; }
        public LifeSkillCourse? LifeSkillCourse { get; set; }
    }
}
