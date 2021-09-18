using Notino.Common.Models;
using System.Threading.Tasks;

namespace Notino.Data
{
    public interface IFileReader
    {
        public string Read(string filePath);
        public FileType GetFileType(string filePath);
    }
}
