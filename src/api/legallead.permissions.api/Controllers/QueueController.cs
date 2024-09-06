using legallead.permissions.api.Entities;
using legallead.permissions.api.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace legallead.permissions.api.Controllers
{
    /// <summary>
    /// Supports messaging operations associated to record search generation processes
    /// </summary>
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
        public async Task<IActionResult> UpdateAsync(QueueUpdateRequest request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.OK };
            if (!request.CanExecute()) return InvalidPayloadResult(message);
            var response = (await _statusSvc.UpdateAsync(request)) ?? new();
            message.Message = response.ToJsonString();
            return new JsonResult(message) { StatusCode = 200 };
        }

        [HttpPost("fetch")]
        public async Task<IActionResult> FetchAsync(ApplicationRequestModel request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.BadRequest };
            if (!IsNameValid(request.Name))
            {
                message.Message = invalidapplicationmessage;
                return new JsonResult(message) { StatusCode = 400 };
            }
            var response = await _statusSvc.FetchAsync();
            message.Message = response.ToJsonString();
            message.StatusCode = (int)HttpStatusCode.OK;
            return new JsonResult(message) { StatusCode = 200 };
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartAsync(QueuedRecord request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.OK };
            if (!request.CanExecute()) return InvalidPayloadResult(message);
            var response = await _statusSvc.StartAsync(request);
            message.Message = response.ToJsonString();
            return new JsonResult(message) { StatusCode = 200 };
        }

        [HttpPost("status")]
        public async Task<IActionResult> StatusAsync(QueueRecordStatusRequest request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.OK };
            if (!request.CanExecute()) return InvalidPayloadResult(message);
            await _statusSvc.PostStatusAsync(request);
            return new JsonResult(message) { StatusCode = 200 };
        }

        [HttpPost("complete")]
        public async Task<IActionResult> CompleteAsync(QueueRecordStatusRequest request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.OK };
            if (!request.CanExecute()) return InvalidPayloadResult(message);
            await _statusSvc.CompleteAsync(request);
            return new JsonResult(message) { StatusCode = 200 };
        }
        [HttpPost("save")]
        public async Task<IActionResult> SaveAsync(QueuePersistenceRequest request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.OK };
            if (request.Content == null || !request.IsValid() || !request.CanExecute()) return InvalidPayloadResult(message);
            await _statusSvc.ContentAsync(request.Id, request.Content);
            return new JsonResult(message) { StatusCode = 200 };
        }

        [HttpPost("finalize")]
        public async Task<IActionResult> FinalizeAsync(QueueCompletionRequest request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.OK };
            if (!request.CanExecute()) return InvalidPayloadResult(message);
            await _statusSvc.GenerationCompleteAsync(request);
            return new JsonResult(message) { StatusCode = 200 };
        }



        [HttpPost("queue-status")]
        public async Task<IActionResult> GetQueueStatusAsync(QueueSummaryRequest request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.BadRequest };
            var response = await _statusSvc.GetQueueStatusAsync(request);
            message.Message = response.ToJsonString();
            message.StatusCode = (int)HttpStatusCode.OK;
            return new JsonResult(message) { StatusCode = 200 };
        }

        [HttpPost("queue-summary")]
        public async Task<IActionResult> GetQueueSummaryAsync(QueueSummaryRequest request)
        {
            var applicationCheck = Request.Validate(invalidapplicationmessage);
            if (!applicationCheck.Key) { return BadRequest(applicationCheck.Value); }
            var message = new QueueRecordResponse { StatusCode = (int)HttpStatusCode.BadRequest };
            var response = await _statusSvc.GetQueueSummaryAsync(request);
            message.Message = response.ToJsonString();
            message.StatusCode = (int)HttpStatusCode.OK;
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
