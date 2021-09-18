using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        public async Task<IActionResult> DownloadFile([FromQuery] string path)
        {


            return Ok();
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadFromFile([FromForm] FileDto file)
        {
            await _fileService.SaveFile(file).ConfigureAwait(false);

            return Ok();
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
        public async Task<IActionResult> UploadFromUrl([FromForm] string path)
        {


            return Ok();
        }
    }
}
