using Notino.Common.Models;
using System.Threading.Tasks;

namespace Notino.Common.Service
{
    public interface IFileService
    {
        public Task SaveFile(FileDto file);
        public Task<Response> Convert(string filePath, FileType desiredType);
    }
}
