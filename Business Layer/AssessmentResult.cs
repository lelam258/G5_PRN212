using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class AssessmentResult
    {
        [Key]
        public int ResultId { get; set; }
        [ForeignKey("Assessment")]
        public required int AssessmentId { get; set; }
        [ForeignKey("Student")]
        public required int StudentId { get; set; }
        public decimal? Score { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public string? SubmissionFilePath { get; set; }
        public Assessment? Assessment { get; set; }
        public Student? Student { get; set; }
    }
}
