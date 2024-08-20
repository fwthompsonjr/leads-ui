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
            if (!request.CanExecute()) return InvalidPayloadResult(message);
            var response = _statusSvc.Insert(request);
            message.Message = response.ToJsonString();
            return new JsonResult(message) { StatusCode = 200 };
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(QueueUpdateRequest request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.OK };
            if (!request.CanExecute()) return InvalidPayloadResult(message);
            var response = (await _statusSvc.Update(request)) ?? new();
            message.Message = response.ToJsonString();
            return new JsonResult(message) { StatusCode = 200 };
        }

        [HttpPost("fetch")]
        public async Task<IActionResult> Fetch(ApplicationRequestModel request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.BadRequest };
            if (!IsNameValid(request.Name))
            {
                message.Message = invalidapplicationmessage;
                return new JsonResult(message) { StatusCode = 400 };
            }
            var response = await _statusSvc.Fetch();
            message.Message = response.ToJsonString();
            return new JsonResult(message) { StatusCode = 200 };
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start(QueuedRecord request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.OK };
            if (!request.CanExecute()) return InvalidPayloadResult(message);
            var response = await _statusSvc.Start(request);
            message.Message = response.ToJsonString();
            return new JsonResult(message) { StatusCode = 200 };
        }

        [HttpPost("status")]
        public async Task<IActionResult> Status(QueueRecordStatusRequest request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.OK };
            if (!request.CanExecute()) return InvalidPayloadResult(message);
            await _statusSvc.PostStatus(request);
            return new JsonResult(message) { StatusCode = 200 };
        }

        [HttpPost("complete")]
        public async Task<IActionResult> Complete(QueueRecordStatusRequest request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.OK };
            if (!request.CanExecute()) return InvalidPayloadResult(message);
            await _statusSvc.Complete(request);
            return new JsonResult(message) { StatusCode = 200 };
        }

        [HttpPost("finalize")]
        public async Task<IActionResult> Finalize(QueueCompletionRequest request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.OK };
            if (!request.CanExecute()) return InvalidPayloadResult(message);
            await _statusSvc.GenerationComplete(request);
            return new JsonResult(message) { StatusCode = 200 };
        }


        private static JsonResult InvalidPayloadResult(QueueRecordResponse response)
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            response.Message = "Invalid action parameter";
            return new JsonResult(response) { StatusCode = 401 };
        }

        private static bool IsNameValid(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            var names = ApplicationModel.GetApplicationsFallback()
                .Select(x => x.Name).ToList();
            return names.Contains(name, StringComparer.Ordinal);
        }

        private const string invalidapplicationmessage = "Invalid application identity";
    }
}
