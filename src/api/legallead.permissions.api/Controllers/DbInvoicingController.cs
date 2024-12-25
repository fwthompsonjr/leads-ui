using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace legallead.permissions.api.Controllers
{
    [Route("/db-invoice")]
    [ApiController]
    public class DbInvoicingController(
    ILeadInvoiceService lead) : ControllerBase
    {
        private readonly ILeadInvoiceService _leadService = lead;
        private static readonly string[] sourceArray = ["Customer", "Invoice"];

        [HttpPost("get-invoice-list")]
        public async Task<IActionResult> GetInvoicesAsync(GetInvoiceRequest request)
        {
            StringComparer ooic = StringComparer.OrdinalIgnoreCase;
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            var allowed = sourceArray.ToList();
            if (!allowed.Contains(request.RequestType, ooic)) 
                return BadRequest($"Invoice search request type: {request.RequestType}");
            var index = allowed.FindIndex(x => x.Equals(request.RequestType, oic));
            var id = index == 0 ? request.CustomerId : request.InvoiceId;
            if (string.IsNullOrEmpty(id))
                return BadRequest($"Requested search index for {request.RequestType} is not provided");
            var response = index == 0 ?
                await _leadService.GetByCustomerIdAsync(id) :
                await _leadService.GetByInvoiceIdAsync(id);
            return Ok(response ?? new());
        }
    }
}