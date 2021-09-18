using Notino.Common.Models;
using System.Threading.Tasks;

namespace Notino.Common.Service.FileConvert
{
    public interface IFileConverter
    {
        public Task<Response> Convert(string filePath, FileType sourceType, FileType desiredType); 
    }
}
