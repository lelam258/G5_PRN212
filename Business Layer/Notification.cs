using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        [ForeignKey("Student")]
        public int? StudentId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Student? Student { get; set; }
    }
}
