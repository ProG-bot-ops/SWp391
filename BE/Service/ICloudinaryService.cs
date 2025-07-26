namespace SWP391_SE1914_ManageHospital.Service
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
