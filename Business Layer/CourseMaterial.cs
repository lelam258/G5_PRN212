using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class CourseMaterial
    {
        [Key]
        public int MaterialId { get; set; }
        [ForeignKey("LifeSkillCourse")]
        public required int CourseId { get; set; }
        public required string Title { get; set; }
        public required string FilePath { get; set; }
        public required DateTime UploadDate { get; set; }
        public LifeSkillCourse? LifeSkillCourse { get; set; }
    }
}
