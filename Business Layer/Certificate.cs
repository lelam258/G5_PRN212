using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class Certificate
    {
        [Key]
        public int CertificateId { get; set; }
        [ForeignKey("Student")]
        public required int StudentId { get; set; }
        [ForeignKey("LifeSkillCourse")]
        public required int CourseId { get; set; }
        public required DateTime IssueDate { get; set; }
        public required string CertificateCode { get; set; }
        public required string FilePath { get; set; }
        public Student? Student { get; set; }
        public LifeSkillCourse? LifeSkillCourse { get; set; }
    }
}
