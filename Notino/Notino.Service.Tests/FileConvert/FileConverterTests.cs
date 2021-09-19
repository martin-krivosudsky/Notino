using Moq;
using Notino.Common.Models;
using Notino.Common.Service.FileConvert;
using Notino.Data;
using Notino.Service.FileConvert;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notino.Service.Tests.FileConvert
{
    public class FileConverterTests
    {
        FileConverter fileConverter;
        private Mock<IConverter> _jsonToXmlConverterMock;
        private Mock<IConverter> _xmlToJsonConverterMock;
        private Mock<IFileReader> _fileReaderMock;
        private Mock<IFileWriter> _fileWriterMock;
        private const string _filePath = "folder/file.xml";

        [SetUp]
        public void Setup()
        {
            _jsonToXmlConverterMock = new();
            _xmlToJsonConverterMock = new();
            var converters = new List<IConverter>() { _jsonToXmlConverterMock.Object, _xmlToJsonConverterMock.Object };
            _fileReaderMock = new();
            _fileWriterMock = new();

            fileConverter = new(converters, _fileReaderMock.Object, _fileWriterMock.Object);
        }

        [Test]
        public async Task Convert_UnsupportedConverter_ReturnsError()
        {
            _jsonToXmlConverterMock.Setup(c => c.IsValid(It.IsAny<FileType>(), It.IsAny<FileType>())).Returns(false);
            _xmlToJsonConverterMock.Setup(c => c.IsValid(It.IsAny<FileType>(), It.IsAny<FileType>())).Returns(false);

            Response response = await fileConverter.Convert(_filePath, FileType.Json, FileType.Xml).ConfigureAwait(false);

            Assert.AreEqual(ResponseCode.ConversionNotSupported, response.ResponseCode);
            _fileWriterMock.Verify(fw => fw.WriteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<bool>()), Times.Never);
        }

        [Test]
        public async Task Convert_HappyPath_WriterCalled()
        {
            byte[] expectedBytes = Encoding.UTF8.GetBytes(FilesToConvert.JsonFile);

            _jsonToXmlConverterMock.Setup(c => c.IsValid(It.IsAny<FileType>(), It.IsAny<FileType>())).Returns(false);
            _xmlToJsonConverterMock.Setup(c => c.IsValid(FileType.Xml, FileType.Json)).Returns(true);
            _fileReaderMock.Setup(fr => fr.ReadText(_filePath)).Returns(FilesToConvert.XmlFile);
            _xmlToJsonConverterMock.Setup(c => c.Convert(FilesToConvert.XmlFile)).Returns(FilesToConvert.JsonFile);

            Response response = await fileConverter.Convert(_filePath, FileType.Xml, FileType.Json).ConfigureAwait(false);


            Assert.AreEqual(ResponseCode.Success, response.ResponseCode);
            _fileWriterMock.Verify(fw => fw.WriteAsync(string.Empty, _filePath.Replace("xml", "json"), It.Is<byte[]>(b => b.SequenceEqual(expectedBytes)), false), Times.Once);
        }
    }
}
