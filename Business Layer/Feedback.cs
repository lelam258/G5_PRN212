using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }
        [ForeignKey("Student")]
        public required int StudentId { get; set; }
        [ForeignKey("LifeSkillCourse")]
        public required int CourseId { get; set; }
        public required int Rating { get; set; }
        public string? Comment { get; set; }
        public required DateTime FeedbackDate { get; set; }
        public Student? Student { get; set; }
        public LifeSkillCourse? LifeSkillCourse { get; set; }
    }
}
