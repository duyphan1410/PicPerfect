using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PicPerfect.Models
{
    public class Reports
    {
        [Key]
        public int ReportId { get; set; }
        public string? ReportConten { get; set;}
        [ForeignKey(nameof(Users))]
        public int UserId {  get; set; }
        public string? ReportTime { get; set; }
        public int Status { get; set; }
    }
}
