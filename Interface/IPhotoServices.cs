using CloudinaryDotNet.Actions;

namespace PicPerfect.Interface
{
    public interface IPhotoServices
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicUrl);
        Task<ImageUploadResult> UpdatePhotoAsync(IFormFile file, string publicId);
    }
}
