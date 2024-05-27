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
        public string UploadDatetime { get; set; }
        public string ImagePath { get; set; }
        [NotMapped] // Không cần ánh xạ thuộc tính này với cột trong cơ sở dữ liệu
        public string ImageName
        {
            get
            {
                // Tạo tên hình ảnh dựa trên số lượng hình ảnh đã lưu
                int imageCount = _context.Images.Count(img => img.UserId == UserId);
                return $"image{imageCount + 1}";
            }
        }

    }
}
