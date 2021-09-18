using System.Threading.Tasks;

namespace Notino.Data
{
    public interface IFileWriter
    {
        public Task WriteAsync(string folderPath, string fileName, byte[] data, bool createFolders);
    }
}
