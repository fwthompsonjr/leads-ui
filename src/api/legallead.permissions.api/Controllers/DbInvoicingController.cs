using legallead.permissions.api.Extensions;
using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace legallead.permissions.api.Controllers
{
    [Route("/db-invoice")]
    [ApiController]
    public class DbInvoicingController(
    ILeadInvoiceService lead,
    ILeadAuthenicationService auth) : ControllerBase
    {
        private readonly ILeadInvoiceService _leadService = lead;
        private readonly ILeadAuthenicationService _authService = auth;
        private static readonly string[] sourceArray = ["Customer", "Invoice"];

        [HttpPost("preview-invoice")]
        public async Task<IActionResult> PreviewInvoiceAsync(GetInvoiceRequest request)
        {
            var allowed = sourceArray.ToList();
            if (!allowed.Contains(request.RequestType, ooic))
                return BadRequest($"Invoice search request type: {request.RequestType}");
            var index = allowed.FindIndex(x => x.Equals(request.RequestType, oic));
            if (index == 0)
                return BadRequest($"Invoice search request type: {request.RequestType}");
            var id = request.InvoiceId;
            var response = await _leadService.GetByInvoiceIdAsync(id);
            if (response == null || response.Headers.Count == 0 || response.Lines.Count == 0)
                return StatusCode(404);
            var content = response.GetHtml();
            return Content(content, "text/html");
        }

        [HttpPost("create-customer-invoice")]
        public async Task<IActionResult> CreateRemoteInvoiceAsync(GetInvoiceRequest request)
        {
            var response = await _leadService.CreateRemoteInvoiceAsync(request);
            return Ok(response ?? new());
        }

        [HttpPost("get-invoice-list")]
        public async Task<IActionResult> GetInvoicesAsync(GetInvoiceRequest request)
        {
            var allowed = sourceArray.ToList();
            if (!allowed.Contains(request.RequestType, ooic))
                return BadRequest($"Invoice search request type: {request.RequestType}");
            var index = allowed.FindIndex(x => x.Equals(request.RequestType, oic));
            var id = index == 0 ? request.CustomerId : request.InvoiceId;
            if (string.IsNullOrEmpty(id))
                return BadRequest($"Requested search index for {request.RequestType} is not provided");
            var response = (index == 0 ?
                await _leadService.GetByCustomerIdAsync(id) :
                await _leadService.GetByInvoiceIdAsync(id)) ?? new();
            if (index == 0 && HasZeroInvoice(Request, response))
            {
                RebuildInvoices(request, response);
            }
            return Ok(response);
        }

        private void RebuildInvoices(GetInvoiceRequest request, GetInvoiceResponse response)
        {
            // throw new NotImplementedException();
        }

        private bool HasZeroInvoice(HttpRequest request, GetInvoiceResponse response)
        {
            var user = _authService.GetUserModel(request, UserAccountAccess);
            if (user == null) return false;
            var hasZero = response.Headers.Any(x => x.InvoiceTotal.GetValueOrDefault() < 0.02m);
            if (!hasZero) return false;
            Debug.WriteLine(user.CountyData);
            return false;
        }

        [HttpPost("get-invoice-status")]
        public async Task<IActionResult> GetInvoiceStatusAsync(GetInvoiceRequest request)
        {
            var allowed = sourceArray.ToList();
            if (!allowed.Contains(request.RequestType, ooic))
                return BadRequest($"Invoice search request type: {request.RequestType}");
            var index = allowed.FindIndex(x => x.Equals(request.RequestType, oic));
            if (index == 0)
                return BadRequest($"Invoice search request type: {request.RequestType}");
            var id = request.InvoiceId;
            var response = await _leadService.GetByInvoiceIdAsync(id);
            if (response == null || response.Headers.Count == 0 || response.Lines.Count == 0)
                return StatusCode(404);
            var header = response.Headers[0];
            if (header.InvoiceNbr != "SENT") return Ok(new { Id = id, Status = header.InvoiceNbr });
            var remoteStatus = await _leadService.GetInvoiceStatusAsync(request);
            return Ok(new { Id = id, Status = remoteStatus });
        }

        private static readonly StringComparer ooic = StringComparer.OrdinalIgnoreCase;
        private const StringComparison oic = StringComparison.OrdinalIgnoreCase;
        private const string UserAccountAccess = "user account access credential";
    }
}