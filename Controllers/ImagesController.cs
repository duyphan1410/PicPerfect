using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PicPerfect.DATA;
using PicPerfect.Models;
using PicPerfect.Interface;
using Microsoft.AspNetCore.Http;

namespace PicPerfect.Controllers
{
    public class ImagesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPhotoServices _photoServices;

        public ImagesController(AppDbContext context, IPhotoServices photoServices)
        {
            _context = context;
            _photoServices = photoServices;
        }

        // POST: Images/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var image = await _context.Images.FindAsync(id);
                if (image == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy ảnh" });
                }

                // Xóa ảnh khỏi Cloudinary
                var deletionResult = await _photoServices.DeletePhotoAsync(image.ImagePath);
                if (deletionResult.Error != null)
                {
                    return Json(new { success = false, message = "Lỗi khi xóa ảnh khỏi cloud: " + deletionResult.Error.Message });
                }

                // Xóa ảnh khỏi database
                _context.Images.Remove(image);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Xóa ảnh thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
}