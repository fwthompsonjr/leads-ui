using legallead.permissions.api.Entities;
using legallead.permissions.api.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueueController(IQueueStatusService statusService) : ControllerBase
    {
        private readonly IQueueStatusService _statusSvc = statusService;

        [HttpPost("initialize")]
        public IActionResult Initialize(QueueInitializeRequest request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.OK };
            var response = _statusSvc.Insert(request);
            message.Message = response.ToJsonString();
            return new JsonResult(message);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(QueueUpdateRequest request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.OK };
            var response = (await _statusSvc.Update(request)) ?? new();
            message.Message = response.ToJsonString();
            return new JsonResult(message);
        }

        [HttpPost("fetch")]
        public async Task<IActionResult> Fetch(ApplicationRequestModel request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.BadRequest };
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return new JsonResult(message);
            }
            var response = await _statusSvc.Fetch();
            message.Message = response.ToJsonString();
            return new JsonResult(message);
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start(QueuedRecord request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.OK };
            var response = await _statusSvc.Start(request);
            message.Message = response.ToJsonString();
            return new JsonResult(message);
        }

        [HttpPost("status")]
        public async Task<IActionResult> Status(QueueRecordStatusRequest request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.OK };
            await _statusSvc.PostStatus(request);
            return new JsonResult(message);
        }

        [HttpPost("complete")]
        public async Task<IActionResult> Complete(QueueRecordStatusRequest request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.OK };
            await _statusSvc.Complete(request);
            return new JsonResult(message);
        }

        [HttpPost("finalize")]
        public async Task<IActionResult> Finalize(QueueCompletionRequest request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.OK };
            await _statusSvc.GenerationComplete(request);
            return new JsonResult(message);
        }

        private const string invalidapplicationmessage = "Invalid application identity";
    }
}
