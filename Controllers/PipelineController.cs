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

        public PipelineController(PipelineService service)
        {
            _service = service;
        }

        [HttpPost("transfer-test")]
        public async Task<IActionResult> TransferDataAsync([FromBody] RequestBody req)
        {
            try
            {
                if (
                    req == null
                    || string.IsNullOrEmpty(req.SourceTableName)
                    || string.IsNullOrEmpty(req.TargetTableName)
                )
                {
                    return BadRequest(
                        ResponseHandler.ToResponse(400, false, null, ["Invalid request payload"])
                    );
                }

                var response = await _service.TransferDataAsync(
                    req.SourceTableName,
                    req.TargetTableName
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    ResponseHandler.ToResponse(500, false, null, ["An error occurred", ex.Message])
                );
            }
        }
    }
}
