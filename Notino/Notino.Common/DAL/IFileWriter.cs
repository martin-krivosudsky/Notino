using Notino.Common.Models;
using System.Threading.Tasks;

namespace Notino.Data
{
    public interface IFileWriter
    {
        public Task<Response> WriteAsync(string folderPath, string fileName, byte[] data, bool createFolders);
        void Delete(string path);
    }
}
