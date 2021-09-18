using Notino.Common;
using Notino.Common.Models;
using Notino.Common.Service;
using Notino.Common.Service.FileConvert;
using Notino.Data;
using System.IO;
using System.Threading.Tasks;

namespace Notino.Services
{
    public class FileService : IFileService
    {
        private readonly IFileWriter _fileWriter;
        private readonly IFileConverter _fileConverter;
        public FileService(IFileWriter fileWriter, IFileConverter fileConverter)
        {
            _fileWriter = fileWriter ?? throw new System.ArgumentNullException(nameof(fileWriter));
            _fileConverter = fileConverter ?? throw new System.ArgumentNullException(nameof(fileConverter));
        }

        public async Task SaveFile(FileDto fileDto)
        {
            string folderPath = Constants.StoragePath + fileDto.Path;

            using MemoryStream memoryStream = new();
            await fileDto.File.CopyToAsync(memoryStream).ConfigureAwait(false);
            var fileBytes = memoryStream.ToArray();
            await _fileWriter.WriteAsync(folderPath, fileDto.File.FileName, fileBytes, true).ConfigureAwait(false);
        }

        public async Task Convert(string filePath, FileType desiredType)
        {
            await _fileConverter.Convert(Constants.StoragePath + filePath, desiredType).ConfigureAwait(false);
        }
    }
}
