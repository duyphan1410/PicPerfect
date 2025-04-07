using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PicPerfect.Models
{
    public class Album
    {
        [Key]
        public int AlbumId { get; set; }
        public string AlbumName { get; set; }
        public string Description { get; set; }
        public string CreationDate { get; set; }
        public int CreatorUserId { get; set; }
        public int NumberOfImage { get; set; }
        public string CoverImage { get; set; }
        public virtual ICollection<AlbumImages> AlbumImages { get; set; }
    }
}
