using Microsoft.AspNetCore.Mvc;
using Notino.Common;
using Notino.Common.Models;
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

        [HttpPost]
        [Route("download")]
        public IActionResult DownloadFile([FromQuery] string path)
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
        public async Task<IActionResult> Convert([FromQuery] string filePath, [FromQuery] FileType desiredType)
        {
            Response response = await _fileService.Convert(filePath, desiredType).ConfigureAwait(false);

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
        [Route("uploadfromurl")]
        public IActionResult UploadFromUrl([FromQuery] string url, [FromQuery] string fileName, [FromQuery] string filePath)
        {
            Response response = _fileService.SaveFileFromUrl(url, filePath, fileName);

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
        [Route("sendbyemail")]
        public IActionResult SendByEmail([FromQuery] string filePath, [FromQuery] string email)
        {
            Response response = _fileService.SendByEmail(filePath, email);

            if (response.ResponseCode == ResponseCode.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(response.ErrorMessage);
            }
        }
    }
}
