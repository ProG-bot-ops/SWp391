using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.IO;

namespace SWP391_SE1914_ManageHospital.Service.Impl
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new Exception("No file uploaded.");
            }

            // Kiểm tra định dạng file (nên tải lên ảnh)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new Exception("Invalid file type. Only image files are allowed.");
            }

            // Tạo tên file duy nhất
            var fileName = Guid.NewGuid().ToString() + fileExtension;
            var fileStream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, fileStream),
                Transformation = new Transformation().Height(400).Width(400).Crop("fill").Gravity("face")
            };

            try
            {
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return uploadResult.SecureUrl.ToString(); 
                }
                else
                {
                    throw new Exception("Error uploading image. " + uploadResult.Error?.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error uploading image: " + ex.Message);
            }
        }
    }
}
