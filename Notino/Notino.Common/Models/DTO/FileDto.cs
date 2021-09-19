using Microsoft.AspNetCore.Http;

namespace Notino.Common.Models.DTO
{
    public class FileDto
    {
        public string Path { get; set; }
        public IFormFile File { get; set; }
    }
}
