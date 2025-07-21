using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class ActivityLog
    {
        [Key]
        public int LogId { get; set; }
        [ForeignKey("Student")]
        public required int UserId { get; set; }
        public required string Action { get; set; }
        public required DateTime Timestamp { get; set; }
        public Student? Student { get; set; }
    }
}
