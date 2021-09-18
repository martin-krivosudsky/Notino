using Microsoft.Extensions.Logging;
using Notino.Common;
using Notino.Common.Models;
using Notino.Common.Service;
using Notino.Common.Service.FileConvert;
using Notino.Data;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Notino.Services
{
    public class FileService : IFileService
    {
        private readonly IFileWriter _fileWriter;
        private readonly IFileReader _fileReader;
        private readonly IFileConverter _fileConverter;
        private readonly IMailService _mailService;
        private readonly WebClient _webClient;
        private readonly ILogger<FileService> _logger;

        public FileService(
            IFileWriter fileWriter,
            IFileConverter fileConverter,
            IFileReader fileReader,
            IMailService mailService,
            ILogger<FileService> logger)
        {
            _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
            _fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));
            _fileConverter = fileConverter ?? throw new ArgumentNullException(nameof(fileConverter));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _webClient = new WebClient();
        }

        public async Task<Response> SaveFile(FileDto fileDto)
        {
            string folderPath = Constants.StoragePath + fileDto.Path;

            using MemoryStream memoryStream = new();
            await fileDto.File.CopyToAsync(memoryStream).ConfigureAwait(false);
            var fileBytes = memoryStream.ToArray();

            return await _fileWriter.WriteAsync(folderPath, fileDto.File.FileName, fileBytes, true).ConfigureAwait(false);
        }

        public async Task<Response> Convert(string filePath, FileType desiredType)
        {
            if (!FileExist(filePath))
            {
                return new Response
                {
                    ResponseCode = ResponseCode.FileNotFound,
                    ErrorMessage = $"File {filePath} not found or not accessible."
                };
            }

            string fileExtension = _fileReader.GetFileExtension(filePath);

            if (!Enum.TryParse(fileExtension, true, out FileType sourceType))
            {
                return new Response
                {
                    ResponseCode = ResponseCode.ConversionNotSupported,
                    ErrorMessage = $"Source file {filePath} type not recognized."
                };
            }

            if (sourceType == desiredType)
            {
                return new Response
                {
                    ResponseCode = ResponseCode.FileAlreadyInDesiredFormat,
                    ErrorMessage = $"File {filePath} is already in desired format."
                };
            }

            return await _fileConverter.Convert(Constants.StoragePath + filePath, sourceType, desiredType).ConfigureAwait(false);
        }

        public Response SaveFileFromUrl(string url, string filePath, string fileName)
        {
            byte[] data;

            try
            {
                data = _webClient.DownloadData(url);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Could not download file from {url}.");

                return new Response
                {
                    ResponseCode = ResponseCode.HttpError,
                    ErrorMessage = $"Could not download file from {url}."
                };
            }

            _fileWriter.WriteAsync(Constants.StoragePath + filePath, fileName, data, true);

            return new Response
            {
                ResponseCode = ResponseCode.Success
            };
        }

        public byte[] GetFile(string path)
        {
            return _fileReader.ReadBytes(path);
        }

        public bool FileExist(string filePath)
        {
            return File.Exists(filePath);
        }

        public Response SendByEmail(string filePath, string email)
        {
            if (!FileExist(filePath))
            {
                return new Response
                {
                    ResponseCode = ResponseCode.FileNotFound,
                    ErrorMessage = $"File {filePath} not found or not accessible."
                };
            }

            byte[] file = GetFile(filePath);

            try
            {
                _mailService.SendMail(email, file, Path.GetFileName(filePath));
            }
            catch(Exception e)
            {
                _logger.LogError(e, $"Could not send email to {email}.");

                return new Response
                {
                    ResponseCode = ResponseCode.MailError,
                    ErrorMessage = $"Could not send email to {email}."
                };
            }

            return new Response
            {
                ResponseCode = ResponseCode.Success
            };
        }
    }
}
