using Notino.Common.Helpers;
using Notino.Common.Models;
using Notino.Common.Service.FileConvert;
using Notino.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notino.Service.FileConvert
{
    public class FileConverter : IFileConverter
    {
        private readonly IEnumerable<IConverter> _converters;
        private readonly IFileReader _fileReader;
        private readonly IFileWriter _fileWriter;

        public FileConverter(IEnumerable<IConverter> converters, IFileReader fileReader, IFileWriter fileWriter)
        {
            _converters = converters ?? throw new System.ArgumentNullException(nameof(converters));
            _fileReader = fileReader ?? throw new System.ArgumentNullException(nameof(fileReader));
            _fileWriter = fileWriter ?? throw new System.ArgumentNullException(nameof(fileWriter));
        }

        public async Task Convert(string filePath, FileType desiredType)
        {
            FileType sourceType = _fileReader.GetFileType(filePath);
            if (sourceType == desiredType)
            {
                //TODO
                return;
            }

            IConverter converter = _converters.FirstOrDefault(c => c.IsValid(sourceType, desiredType));
            if (converter == null)
            {
                //TODO
                return;
            }

            string source = _fileReader.Read(filePath);

            string converted = converter.Convert(source);

            byte[] desiredFile = Encoding.UTF8.GetBytes(converted);

            string newFilePath = FilenameHelper.RenameExtension(filePath, sourceType.ToString().ToLower(), desiredType.ToString().ToLower());
            await _fileWriter.WriteAsync(string.Empty, newFilePath, desiredFile, false).ConfigureAwait(false);
        }
    }
}
