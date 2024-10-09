using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PipelineDataFlow.Models;
using PipelineDataFlow.Services;
using PipelineDataFlow.Utils.Handler;

namespace PipelineDataFlow.Controllers
{
    [ApiController]
    [Route("pipeline")]
    public class PipelineController : ControllerBase
    {
        private readonly PipelineService _service;

        public PipelineController( PipelineService service)
        {
            _service = service;
        }

        [HttpPost("transfer-test")]
        public async Task<IActionResult> TransferDataAsync([FromBody] RequestBody req)
        {
            if (
                req == null
                || string.IsNullOrEmpty(req.SourceTableName)
                || string.IsNullOrEmpty(req.TargetTableName)
            )
            {
                return BadRequest("Invalid request payload");
            }
z
            var response = await _service.TransferDataAsync(
                req.SourceTableName,
                req.TargetTableName
            );
            return Ok(response);
        }
    }
}
