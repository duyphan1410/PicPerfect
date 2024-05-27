using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PicPerfect.Models
{
    public class EditHistory
    {
        [Key]
        public int EditId { get; set; }
        [ForeignKey(nameof(Images))]
        public int ImageId { get; set; }
        public string EditDescription { get; set; }
        public string EditDatetime { get; set; }
    }
}
