using Microsoft.Extensions.Logging;
using Notino.Common.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Notino.Data
{
    public class FileWriter : IFileWriter
    {
        private readonly ILogger<FileWriter> _logger;

        public FileWriter(ILogger<FileWriter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Delete(string path)
        {
            File.Delete(path);
        }

        public async Task<Response> WriteAsync(string folderPath, string fileName, byte[] data, bool createFolders)
        {
            string filePath = folderPath + fileName;

            if(File.Exists(filePath))
            {
                return new Response
                {
                    ResponseCode = ResponseCode.FileAlreadyExists,
                    ErrorMessage = "File with that name already exists."
                };
            }

            try
            {
                if (createFolders)
                {
                    Directory.CreateDirectory(folderPath);
                }

                using (FileStream sourceStream = new(filePath,
                    FileMode.CreateNew, FileAccess.Write, FileShare.None,
                    bufferSize: 4096, useAsync: true))
                {
                    await sourceStream.WriteAsync(data.AsMemory(0, data.Length)).ConfigureAwait(false);
                };

                return new Response
                {
                    ResponseCode = ResponseCode.Success
                };
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Errror occured while trying to create file.");

                return new Response
                {
                    ResponseCode = ResponseCode.ErrorWhileWritingToFile,
                    ErrorMessage = "Errror occured while trying to create file."
                };
            }
        }
    }
}
