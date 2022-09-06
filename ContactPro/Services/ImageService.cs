using ContactPro.Services.Interfaces;

namespace ContactPro.Services
{
    public class ImageService : IImageService
    {
        private readonly string[] suffixes = {"Bytes", "KB", "MB", "GB", "TB", "PB"};
        private readonly string defaultImage = "/img/DefaultContactImage.png";
        public string ConvertByteArrayToFile(byte[] fileData, string extension)
        {
            //Summary:
            //If null assign predefined default image
            //Convert file data byte array into base64 string and store it into a variable
            //Format string so the browser will interpret it as the src in an HTML image tag
            //ex : <img src="data:jpeg;base64,imageData"/>

            if (fileData is null) return defaultImage;

            try
            {
                string imageBase64Data = Convert.ToBase64String(fileData);
                return string.Format($"data:{ extension };base64,{ imageBase64Data }");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            //Summary:
            //Open memory stream
            //Copy uploaded file to the opened memory stream
            //Create byte array variable, convert the memory stream to a byte array and store it in the variable
            //return byte array variable

            try
            {
                using MemoryStream memoryStream = new();
                await file.CopyToAsync(memoryStream); 
                byte[] byteFile = memoryStream.ToArray();

                return byteFile;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
