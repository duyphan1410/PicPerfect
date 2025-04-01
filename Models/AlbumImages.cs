using System.ComponentModel.DataAnnotations;

namespace PicPerfect.Models
{
    public class AlbumImages
    {
        public int AlbumId { get; set; }
        public int ImageId { get; set; }
        public virtual Album Album { get; set; }
        public virtual Images Image { get; set; }
    }
}