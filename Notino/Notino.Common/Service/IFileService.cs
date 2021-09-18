using Notino.Common.Models;
using System.Threading.Tasks;

namespace Notino.Common.Service
{
    public interface IFileService
    {
        Task SaveFile(FileDto file);
        Task Convert(string filePath, FileType desiredType);
    }
}
