using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using PicPerfect.Helpers;
using PicPerfect.Interface;

namespace PicPerfect.Services
{
    public class PhotoServices : IPhotoServices
    {
        private readonly Cloudinary _cloudinary;
        public PhotoServices(IOptions<CloudinarySetting> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("Không có file nào được chọn");
                }

                // Kiểm tra kích thước file (ví dụ: tối đa 10MB)
                if (file.Length > 10 * 1024 * 1024)
                {
                    throw new ArgumentException("File quá lớn. Kích thước tối đa là 10MB");
                }

                // Kiểm tra định dạng file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new ArgumentException("Định dạng file không được hỗ trợ. Chỉ chấp nhận: jpg, jpeg, png, gif");
                }

                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    // Transformation = new Transformation().Crop("fill").Gravity("face")
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception($"Lỗi khi upload lên Cloudinary: {uploadResult.Error.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading photo: {ex.Message}");
                throw; // Ném lại exception để xử lý ở controller
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicUrl)
        {
            var deletionResult = new DeletionResult();
            try
            {
                if (string.IsNullOrEmpty(publicUrl))
                {
                    throw new ArgumentException("URL không hợp lệ");
                }

                // Lấy public_id từ URL
                var uri = new Uri(publicUrl);
                var publicId = Path.GetFileNameWithoutExtension(uri.LocalPath);

                deletionResult = await _cloudinary.DestroyAsync(new DeletionParams(publicId));

                if (deletionResult.Error != null)
                {
                    throw new Exception($"Lỗi khi xóa ảnh trên Cloudinary: {deletionResult.Error.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting photo: {ex.Message}");
                throw;
            }
            return deletionResult;
        }
    }
}
