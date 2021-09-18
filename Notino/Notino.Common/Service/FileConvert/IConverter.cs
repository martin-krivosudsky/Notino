using Notino.Common.Models;

namespace Notino.Common.Service.FileConvert
{
    public interface IConverter
    {
        public string Convert(string source);
        public bool IsValid(FileType sourceType, FileType outputType);
    }
}
