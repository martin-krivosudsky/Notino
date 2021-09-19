using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using Notino.Common;
using Notino.Common.Models;
using Notino.Common.Service;
using Notino.Common.Service.FileConvert;
using Notino.Data;
using Notino.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Notino.Service.Tests
{
    public class FileServiceTests
    {
        private FileService _fileService;
        private Mock<IFileWriter> _fileWriterMock;
        private Mock<IFileReader> _fileReaderMock;
        private Mock<IFileConverter> _fileConverterMock;
        private Mock<IMailService> _mailServiceMock;
        private Mock<IWebService> _webServiceMock;
        private Mock<ILogger<FileService>> _loggerMock;

        private const string _filePath = "folder\\file.xml";

        [SetUp]
        public void Setup()
        {
            _fileWriterMock = new();
            _fileReaderMock = new();
            _fileConverterMock = new();
            _mailServiceMock = new();
            _webServiceMock = new();
            _loggerMock = new();

            _fileService = new(_fileWriterMock.Object, _fileConverterMock.Object, _fileReaderMock.Object, _mailServiceMock.Object, _webServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task SaveFile_WriteAsyncCalled()
        {
            var stringStream = new MemoryStream(Encoding.UTF8.GetBytes("pdf file"));
            string fileName = "test.pdf";

            var file = new FormFile(stringStream, 0, stringStream.Length, null, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };

            FileDto fileDto = new()
            {
                Path = _filePath,
                File = file
            };

            await _fileService.SaveFile(fileDto).ConfigureAwait(false);

            byte[] expectedBytes = stringStream.ToArray();
            _fileWriterMock.Verify(fw => fw.WriteAsync(Constants.StoragePath + fileDto.Path, fileName, It.Is<byte[]>(b => b.SequenceEqual(expectedBytes)), true), Times.Once);
        }

        [Test]
        public async Task Convert_FileDoesNotExist_ErrorReturned()
        {
            _fileReaderMock.Setup(fr => fr.FileExists(_filePath)).Returns(false);

            Response response = await _fileService.Convert(_filePath, FileType.Json).ConfigureAwait(false);

            Assert.AreEqual(ResponseCode.FileNotFound, response.ResponseCode);
            _fileConverterMock.Verify(fc => fc.Convert(It.IsAny<string>(), It.IsAny<FileType>(), It.IsAny<FileType>()), Times.Never);
        }

        [Test]
        public async Task Convert_TryingToConvertUnsupportedFile_ErrorReturned()
        {
            _fileReaderMock.Setup(fr => fr.FileExists(_filePath)).Returns(true);
            _fileReaderMock.Setup(fr => fr.GetFileExtension(_filePath)).Returns("pdf");

            Response response = await _fileService.Convert(_filePath, FileType.Json).ConfigureAwait(false);

            _fileConverterMock.Verify(fc => fc.Convert(It.IsAny<string>(), It.IsAny<FileType>(), It.IsAny<FileType>()), Times.Never);
            Assert.AreEqual(ResponseCode.ConversionNotSupported, response.ResponseCode);
        }

        [Test]
        public async Task Convert_FileIsAlreadyConverted_ErrorReturned()
        {
            _fileReaderMock.Setup(fr => fr.FileExists(_filePath)).Returns(true);
            _fileReaderMock.Setup(fr => fr.GetFileExtension(_filePath)).Returns("xml");

            Response response = await _fileService.Convert(_filePath, FileType.Xml).ConfigureAwait(false);

            Assert.AreEqual(ResponseCode.FileAlreadyInDesiredFormat, response.ResponseCode);
            _fileConverterMock.Verify(fc => fc.Convert(It.IsAny<string>(), It.IsAny<FileType>(), It.IsAny<FileType>()), Times.Never);
        }

        [Test]
        public async Task Convert_HappyPath_ConverCalled()
        {
            _fileReaderMock.Setup(fr => fr.FileExists(_filePath)).Returns(true);
            _fileReaderMock.Setup(fr => fr.GetFileExtension(_filePath)).Returns("xml");
            _fileConverterMock.Setup(fc => fc.Convert(Constants.StoragePath + _filePath, FileType.Xml, FileType.Json)).ReturnsAsync(new Response {ResponseCode = ResponseCode.Success });

            Response response = await _fileService.Convert(_filePath, FileType.Json).ConfigureAwait(false);

            _fileConverterMock.Verify(fc => fc.Convert(Constants.StoragePath + _filePath, FileType.Xml, FileType.Json), Times.Once);
            Assert.AreEqual(ResponseCode.Success, response.ResponseCode);
        }

        [Test]
        public async Task SaveFileFromUrl_HappyPath_WriteAsyncCalled()
        {
            byte[] file = new byte[] { 1, 2, 3, 4, 5, 6, 7, 11 };
            string url = "validUrl.com";

            _webServiceMock.Setup(ws => ws.DownloadFile(url)).Returns(file);

            Response response = await _fileService.SaveFileFromUrl(url, string.Empty, _filePath).ConfigureAwait(false);

            _fileWriterMock.Verify(fw => fw.WriteAsync(Constants.StoragePath, _filePath, file, true), Times.Once);
            Assert.AreEqual(ResponseCode.Success, response.ResponseCode);
        }

        [Test]
        public async Task SaveFileFromUrl_InvalidUrl_ErrorReturned()
        {
            string url = "invalidUrl.com";
            WebException exception = new();

            _webServiceMock.Setup(ws => ws.DownloadFile(url)).Throws(exception);

            Response response = await _fileService.SaveFileFromUrl(url, string.Empty, _filePath).ConfigureAwait(false);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals($"Could not download file from {url}.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            Assert.AreEqual(ResponseCode.HttpError, response.ResponseCode);
        }

        [Test]
        public void GetFile_FileReaderCalled()
        {
            _fileService.GetFile(_filePath);

            _fileReaderMock.Verify(fr => fr.ReadBytes(_filePath), Times.Once);
        }

        [Test]
        public void GetFilesInfo_FileReaderCalled()
        {
            _fileService.GetFilesInfo();

            _fileReaderMock.Verify(fr => fr.GetFilesInfo(), Times.Once);
        }

        [Test]
        public void GetFilesInfo_ExceptionThrown_EmptyListReturned()
        {
            IOException exception = new();

            _fileReaderMock.Setup(fr => fr.GetFilesInfo()).Throws(exception);

            IEnumerable<Common.Models.FileInfo> filesInfo = _fileService.GetFilesInfo();

            _fileReaderMock.Verify(fr => fr.GetFilesInfo(), Times.Once);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals("Something went wrong when reading files.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            Assert.IsEmpty(filesInfo);
        }

        [Test]
        public void FileExist_FileReaderCalled()
        {
            _fileService.FileExist(_filePath);

            _fileReaderMock.Verify(fr => fr.FileExists(_filePath), Times.Once);
        }

        [Test]
        public void SendByEmail_FileNotFound_ErrorReturned()
        {
            string email = "test@mail.com";
            _fileReaderMock.Setup(fr => fr.FileExists(_filePath)).Returns(false);

            Response response = _fileService.SendByEmail(_filePath, email);

            _mailServiceMock.Verify(ms => ms.SendMail(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>()), Times.Never);
            Assert.AreEqual(ResponseCode.FileNotFound, response.ResponseCode);
        }

        [Test]
        public void SendByEmail_HappyPath_FileSent()
        {
            string email = "test@mail.com";
            byte[] file = new byte[] { 1, 2, 3, 99 };
            string fileName = "file.xml";

            _fileReaderMock.Setup(fr => fr.FileExists(Constants.StoragePath + _filePath)).Returns(true);
            _fileReaderMock.Setup(fr => fr.ReadBytes(Constants.StoragePath + _filePath)).Returns(file);
            _fileReaderMock.Setup(fr => fr.GetFileName(Constants.StoragePath + _filePath)).Returns(fileName);

            Response response = _fileService.SendByEmail(_filePath, email);

            _mailServiceMock.Verify(ms => ms.SendMail(email, file, fileName), Times.Once);
            Assert.AreEqual(ResponseCode.Success, response.ResponseCode);
        }

        [Test]
        public void SendByEmail_SmtpError_ErrorReturned()
        {
            string email = "test@mail.com";
            byte[] file = new byte[] { 1, 2, 3, 99 };
            string fileName = "file.xml";
            SmtpException exception = new();

            _fileReaderMock.Setup(fr => fr.FileExists(Constants.StoragePath + _filePath)).Returns(true);
            _fileReaderMock.Setup(fr => fr.ReadBytes(Constants.StoragePath + _filePath)).Returns(file);
            _fileReaderMock.Setup(fr => fr.GetFileName(Constants.StoragePath + _filePath)).Returns(fileName);
            _mailServiceMock.Setup(ms => ms.SendMail(email, file, fileName)).Throws(exception);

            Response response = _fileService.SendByEmail(_filePath, email);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals($"Could not send email to {email}.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _mailServiceMock.Verify(ms => ms.SendMail(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(ResponseCode.MailError, response.ResponseCode);
        }
    }
}