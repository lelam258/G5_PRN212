using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }
        [ForeignKey("Student")]
        public required int StudentId { get; set; }
        [ForeignKey("LifeSkillCourse")]
        public required int CourseId { get; set; }
        public required bool CompletionStatus { get; set; }
        public DateTime? CompletionDate { get; set; }
        public Student? Student { get; set; }
        public LifeSkillCourse? Course { get; set; }
    }
}
