using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class CourseSchedule
    {
        [Key]
        public int ScheduleId { get; set; }
        [ForeignKey("LifeSkillCourse")]
        public int CourseId { get; set; }
        public required DateTime SessionDate { get; set; }
        public required TimeSpan StartTime { get; set; }
        public required TimeSpan EndTime { get; set; }
        public required string Room { get; set; }
        public LifeSkillCourse? LifeSkillCourse { get; set; }
    }
}
