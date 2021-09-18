using Notino.Common;
using Notino.Common.Models;
using Notino.Common.Service;
using Notino.Common.Service.FileConvert;
using Notino.Data;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Notino.Services
{
    public class FileService : IFileService
    {
        private readonly IFileWriter _fileWriter;
        private readonly IFileReader _fileReader;
        private readonly IFileConverter _fileConverter;
        public FileService(IFileWriter fileWriter, IFileConverter fileConverter, IFileReader fileReader)
        {
            _fileWriter = fileWriter ?? throw new System.ArgumentNullException(nameof(fileWriter));
            _fileReader = fileReader ?? throw new System.ArgumentNullException(nameof(fileReader));
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

        public async Task<Response> Convert(string filePath, FileType desiredType)
        {
            string fileExtension = _fileReader.GetFileExtension(filePath);

            if (!Enum.IsDefined(typeof(FileType), fileExtension))
            {
                return new Response
                {
                    ResponseCode = ResponseCode.ConversionNotSupported,
                    ErrorMessage = "Source file type not recognized."
                };
            }
            FileType sourceType = (FileType)Enum.Parse(typeof(FileType), fileExtension);

            if (sourceType == desiredType)
            {
                return new Response
                {
                    ResponseCode = ResponseCode.FileAlreadyInDesiredFormat,
                    ErrorMessage = "File already in desired format."
                };
            }

            return await _fileConverter.Convert(Constants.StoragePath + filePath, sourceType, desiredType).ConfigureAwait(false);
        }
    }
}
