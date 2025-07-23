using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class Assessment
    {
        [Key]
        public int AssessmentId { get; set; }
        [ForeignKey("LifeSkillCourse")]
        public required int CourseId { get; set; }
        public required string AssessmentName { get; set; }
        public required int MaxScore { get; set; }
        public required DateTime DueDate { get; set; }
        public string? AssessmentType { get; set; }
        public string? Instructions { get; set; }
        public LifeSkillCourse? LifeSkillCourse { get; set; }
    }
}
