using Microsoft.EntityFrameworkCore;
using PicPerfect.DATA;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PicPerfect.Models
{
    public class Images
    {
        private readonly AppDbContext _context;
        [Key]
        public int ImageId { get; set; }
        [ForeignKey(nameof(Users))]
        public int UserId { get; set; }
        public string? UploadDatetime { get; set; }
        public required string ImagePath { get; set; }
        public string? ImageName { get; set; }
    }
}
