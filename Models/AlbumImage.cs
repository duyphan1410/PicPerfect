using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PicPerfect.Models
{
    public class AlbumImage
    {
        [Key, Column(Order = 0)]
        public int AlbumId { get; set; }

        [Key, Column(Order = 1)]
        public int ImageId { get; set; }

        [ForeignKey(nameof(AlbumId))]
        public Album? Album { get; set; }

        [ForeignKey(nameof(ImageId))]
        public Images? Image { get; set; }
    }
}
