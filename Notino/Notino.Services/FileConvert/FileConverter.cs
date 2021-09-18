using Notino.Common.Helpers;
using Notino.Common.Models;
using Notino.Common.Service.FileConvert;
using Notino.Data;
using System;
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
            _converters = converters ?? throw new ArgumentNullException(nameof(converters));
            _fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));
            _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        }

        public async Task<Response> Convert(string filePath, FileType sourceType, FileType desiredType)
        {
            IConverter converter = _converters.FirstOrDefault(c => c.IsValid(sourceType, desiredType));
            if (converter == null)
            {
                return new Response
                {
                    ResponseCode = ResponseCode.ConversionNotSupported,
                    ErrorMessage = $"Converter from {sourceType} to {desiredType} not found."                    
                };
            }

            string source = _fileReader.ReadText(filePath);

            string converted = converter.Convert(source);

            byte[] desiredFile = Encoding.UTF8.GetBytes(converted);

            string newFilePath = FilenameHelper.RenameExtension(filePath, sourceType.ToString().ToLower(), desiredType.ToString().ToLower());
            await _fileWriter.WriteAsync(string.Empty, newFilePath, desiredFile, false).ConfigureAwait(false);

            return new Response
            {
                ResponseCode = ResponseCode.Success
            };
        }
    }
}
