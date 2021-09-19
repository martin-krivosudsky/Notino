using Microsoft.AspNetCore.Mvc;
using Notino.Common;
using Notino.Common.Models;
using Notino.Common.Models.DTO;
using Notino.Common.Service;
using System.Threading.Tasks;

namespace Notino.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService ?? throw new System.ArgumentNullException(nameof(fileService));
        }

        [HttpGet]
        [Route("download")]
        public IActionResult Download([FromQuery] string path)
        {
            path = Constants.StoragePath + path;

            if (_fileService.FileExist(path))
            {
                return File(_fileService.GetFile(path), "application/octec-stream");
            }
            else
            {
                return BadRequest("File does not exist or is not accessible.");
            }
        }

        [HttpPost]
        [Route("delete")]
        public IActionResult Delete([FromQuery] string path)
        {
            path = Constants.StoragePath + path;

            if (_fileService.FileExist(path))
            {

                _fileService.DeleteFile(path);
                return Ok();
            }
            else
            {
                return BadRequest("File does not exist or is not accessible.");
            }
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadFromFile([FromForm] FileDto file)
        {
            Response response = await _fileService.SaveFile(file).ConfigureAwait(false);

            if (response.ResponseCode == ResponseCode.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(response.ErrorMessage);
            }
        }

        [HttpPost]
        [Route("convert")]
        public async Task<IActionResult> Convert([FromBody] ConvertDto convertDto)
        {
            Response response = await _fileService.Convert(convertDto.FilePath, convertDto.DesiredType).ConfigureAwait(false);

            if (response.ResponseCode == ResponseCode.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(response.ErrorMessage);
            }
        }

        [HttpPost]
        [Route("upload-from-url")]
        public async Task<IActionResult> UploadFromUrl([FromForm] UploadFromFileDto uploadFromFileDto)
        {
            Response response = await _fileService.SaveFileFromUrl(uploadFromFileDto.Url, uploadFromFileDto.FilePath, uploadFromFileDto.FileName).ConfigureAwait(false);

            if (response.ResponseCode == ResponseCode.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(response.ErrorMessage);
            }
        }

        [HttpPost]
        [Route("send-by-email")]
        public IActionResult SendByEmail([FromForm] SendByEmailDto sendByEmailDto)
        {
            Response response = _fileService.SendByEmail(sendByEmailDto.FilePath, sendByEmailDto.Email);

            if (response.ResponseCode == ResponseCode.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(response.ErrorMessage);
            }
        }

        [HttpGet]
        [Route("get-all")]
        public IActionResult GetAllFilesInStorage()
        {
            return Ok(_fileService.GetFilesInfo());
        }
    }
}
