namespace ContactPro.Services.Interfaces
{
    public interface IImageService
    {
        public Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file);
        public string ConvertByteArrayToFile(byte[] fileData, string extension);
        //public string GetFileExtension(IFormFile file);
        //public string GetFileName(IFormFile file);
    }
}
