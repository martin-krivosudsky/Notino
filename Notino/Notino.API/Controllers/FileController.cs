using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Notino.Common.Models;
using System.Threading.Tasks;

namespace Notino.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly ILogger<FileController> _logger;

        public FileController(ILogger<FileController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> DownloadFile([FromQuery] string path)
        {


            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFromFile([FromForm] FileDto file)
        {


            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFromUrl([FromForm] string path)
        {


            return Ok();
        }
    }
}
