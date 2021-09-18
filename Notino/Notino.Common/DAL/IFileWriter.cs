using System.Threading.Tasks;

namespace Notino.Data
{
    public interface IFileWriter
    {
        public Task WriteAsync(string filePath, byte[] data);
    }
}
