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
                if (file.Length > 0)
                {
                    using var stream = file.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi tải lên hình ảnh ở đây
                // Ví dụ: Ghi log, thông báo người dùng, v.v.
                // Đảm bảo rằng bạn xử lý lỗi một cách an toàn và hợp lý
                Console.WriteLine($"Error uploading photo: {ex.Message}");
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicUrl)
        {
            var deletionResult = new DeletionResult();
            try
            {
                var publicId = publicUrl.Split('/').Last().Split('.')[0];
                var deleteParams = new DeletionParams(publicId);
                deletionResult = await _cloudinary.DestroyAsync(deleteParams);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi xóa hình ảnh ở đây
                // Ví dụ: Ghi log, thông báo người dùng, v.v.
                // Đảm bảo rằng bạn xử lý lỗi một cách an toàn và hợp lý
                Console.WriteLine($"Error deleting photo: {ex.Message}");
            }
            return deletionResult;
        }
    }
}
