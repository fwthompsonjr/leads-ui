using legallead.permissions.api.Entities;
using legallead.permissions.api.Extensions;
using legallead.permissions.api.Models;
using legallead.permissions.api.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace legallead.permissions.api.Controllers
{
    [Route("/db-invoice")]
    [ApiController]
    public class DbInvoicingController(
    ILeadInvoiceService lead,
    ILeadAuthenicationService auth,
    IUserUsageService usage) : ControllerBase
    {
        private readonly ILeadInvoiceService _leadService = lead;
        private readonly ILeadAuthenicationService _authService = auth;
        private readonly IUserUsageService _usageService = usage;
        private static readonly string[] sourceArray = ["Customer", "Invoice"];

        [HttpPost("preview-invoice")]
        public async Task<IActionResult> PreviewInvoiceAsync(GetInvoiceRequest request)
        {
            var mode = GetBillingType(Request);
            var currentApiKey = StripeConfiguration.ApiKey;
            var requestedApiKey = PaymentCodeService.GetCode(mode);
            try
            {
                var allowed = sourceArray.ToList();
                if (!allowed.Contains(request.RequestType, ooic))
                    return BadRequest($"Invoice search request type: {request.RequestType}");
                var index = allowed.FindIndex(x => x.Equals(request.RequestType, oic));
                if (index == 0)
                    return BadRequest($"Invoice search request type: {request.RequestType}");
                var id = request.InvoiceId;
                if (!string.IsNullOrEmpty(requestedApiKey) && !requestedApiKey.Equals(currentApiKey))
                {
                    StripeConfiguration.ApiKey = requestedApiKey;
                }
                var response = await _leadService.GetByInvoiceIdAsync(id);
                if (response == null || response.Headers.Count == 0 || response.Lines.Count == 0)
                    return StatusCode(404);
                var content = response.GetHtml();
                return Content(content, "text/html");
            }
            finally
            {
                StripeConfiguration.ApiKey = currentApiKey;
            }
        }

        [HttpPost("create-customer-invoice")]
        public async Task<IActionResult> CreateRemoteInvoiceAsync(GetInvoiceRequest request)
        {
            var mode = GetBillingType(Request);
            var currentApiKey = StripeConfiguration.ApiKey;
            var requestedApiKey = PaymentCodeService.GetCode(mode);
            try
            {
                if (!string.IsNullOrEmpty(requestedApiKey) && !requestedApiKey.Equals(currentApiKey))
                {
                    StripeConfiguration.ApiKey = requestedApiKey;
                }
                var response = await _leadService.CreateRemoteInvoiceAsync(request);
                return Ok(response ?? new());
            }
            finally
            {
                StripeConfiguration.ApiKey = currentApiKey;
            }
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
                    RebuildInvoices(response);
                    return await GetInvoicesAsync(request);
                }
                return Ok(response);
        }

        [HttpPost("get-invoice-by-tracking-index")]
        public async Task<IActionResult> GetInvoiceByRequestIdAsync(GetInvoiceRequest request)
        {
            var rsp = await GetInvoicesAsync(request);
            if (rsp is not OkObjectResult okObject) return rsp;
            if (okObject.Value is not GetInvoiceResponse invoiceResponse) return BadRequest();
            var found = invoiceResponse.Headers.Find(x => x.RequestId == request.InvoiceId);
            if (found == null) return BadRequest();
            return Ok(new GetInvoiceResponse
            {
                Headers = invoiceResponse.Headers.FindAll(x => x.Id == found.Id),
                Lines = invoiceResponse.Lines.FindAll(x => x.Id == found.Id)
            });
        }

        [HttpPost("get-invoice-status")]
        public async Task<IActionResult> GetInvoiceStatusAsync(GetInvoiceRequest request)
        {
            var mode = GetBillingType(Request);
            var currentApiKey = StripeConfiguration.ApiKey;
            var requestedApiKey = PaymentCodeService.GetCode(mode);
            try
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
                if (!string.IsNullOrEmpty(requestedApiKey) && !requestedApiKey.Equals(currentApiKey))
                {
                    StripeConfiguration.ApiKey = requestedApiKey;
                }
                var remoteStatus = await _leadService.GetInvoiceStatusAsync(request);
                return Ok(new { Id = id, Status = remoteStatus });
            } 
            finally
            {
                StripeConfiguration.ApiKey = currentApiKey;
            }
        }


        [HttpPost("get-billing-mode")]
        public IActionResult GetBillingMode(GetInvoiceRequest request)
        {
            var allowed = sourceArray.ToList();
            if (!allowed.Contains(request.RequestType, ooic))
                return BadRequest($"Invoice search request type: {request.RequestType}");
            var user = GetLead(Request);
            var mode = GetBillingType(Request);
            var obj = new { UserName = user?.UserName ?? "", BillingMode = mode };
            return Ok(obj);
        }

        [HttpPost("set-billing-mode")]
        public async Task<IActionResult> SetBillingModeAsync(SetBillingModeModel model)
        {
            var actual = await _usageService.SetUserBillingTypeAsync(model.Id, model.BillingCode);
            var obj = new { RequestedCode = model.BillingCode, BillingMode = actual };
            return Ok(obj);
        }
        private void RebuildInvoices(GetInvoiceResponse response)
        {
            var items = response.Headers.FindAll(x => x.InvoiceTotal.GetValueOrDefault() < 0.02m)
                .Select(x => x.Id).Distinct().ToList();
            items.ForEach(x =>
            {
                _ = Task.Run(() => { _ = _leadService.RemoveAdminDiscountAsync(x).Result; });
            });

        }

        private bool HasZeroInvoice(HttpRequest request, GetInvoiceResponse response)
        {
            var user = GetLead(request);
            if (user == null) return false;
            var hasZero = response.Headers.Any(x => x.InvoiceTotal.GetValueOrDefault() < 0.02m);
            if (!hasZero) return false;
            var data = _usageService.GetUserAdminStatusAsync(user.Id).Result;
            if (data == null) return false;
            if (data.IsAdmin.GetValueOrDefault()) return false;
            return true;
        }

        private string GetBillingType(HttpRequest request)
        {
            const string billingMode = "TEST";
            var user = GetLead(request);
            if (user == null) return billingMode;
            if (string.IsNullOrWhiteSpace(user.Id)) return billingMode;
            var actual = _usageService.GetUserBillingTypeAsync(user.Id).Result;
            if (string.IsNullOrEmpty(actual)) return billingMode;
            return actual;
        }

        private LeadUserModel? GetLead(HttpRequest request)
        {
            return _authService.GetUserModel(request, UserAccountAccess); ;
        }

        private static readonly StringComparer ooic = StringComparer.OrdinalIgnoreCase;
        private const StringComparison oic = StringComparison.OrdinalIgnoreCase;
        private const string UserAccountAccess = "user account access credential";
    }
}