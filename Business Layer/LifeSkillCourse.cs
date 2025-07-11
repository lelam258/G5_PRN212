using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class LifeSkillCourse
    {
        [Key]
        public int CourseId { get; set; }
        public required string CourseName { get; set; }
        [ForeignKey("Instructor")]
        public required int InstructorId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public int? MaxStudents { get; set; }
        public decimal? Price { get; set; }
        public string? Status { get; set; } 
        public Instructor? Instructor { get; set; }
    }
}
