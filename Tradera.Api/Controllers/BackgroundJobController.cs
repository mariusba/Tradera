using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tradera.Contract;
using Tradera.Services;

namespace Tradera.Api.Controllers
{
    [ApiController]
    [Route("jobs")]
    public class BackgroundJobController : ControllerBase
    {
        private readonly IBackgroundJobService _service;

        public BackgroundJobController(IBackgroundJobService service)
        {
            _service = service;
        }

        [HttpPost("start")]
        public async Task<ActionResult> Start([FromBody] StartTaskRequest request)
        {
            await _service.Start(request);
            return Accepted();
        }

        [HttpPost("stop")]
        public async Task<ActionResult> Stop([FromBody] StopTaskRequest request)
        {
            await _service.Stop(request);
            return Accepted();
        }
    }
}