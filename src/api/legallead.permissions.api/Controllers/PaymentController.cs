using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(
        PaymentStripeOption payment,
        ISearchInfrastructure infra,
        IStripeInfrastructure stripe) : ControllerBase
    {
        private readonly PaymentStripeOption stripeOptions = payment;
        private readonly ISearchInfrastructure infrastructure = infra;
        private readonly IStripeInfrastructure stripeService = stripe;

        [HttpGet("product-codes")]
        public IActionResult ProductCodes()
        {
            return Ok(stripeOptions.Codes);
        }

        [HttpGet]
        public IActionResult SessionStatus([FromQuery] string session_id)
        {
            return Ok(stripeService.SessionStatus(session_id));
        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> Create(PaymentCreateRequest request)
        {
            var user = await infrastructure.GetUser(Request);
            var guid = request.Id;
            if (user == null || !Guid.TryParse(guid, out var _)) { return Unauthorized(); }
            var searches = await infrastructure.GetPreview(Request, guid);
            if (searches == null) return UnprocessableEntity(guid);
            var invoice = await infrastructure.CreateInvoice(user.Id, guid);
            if (invoice == null || !invoice.Any()) return UnprocessableEntity(guid);
            var data = invoice.ToList();
            var model = new PaymentCreateModel(Request, user, guid, request.ProductType);
            var response = await stripeService.CreatePaymentAsync(model, data);
            if (response == null) return UnprocessableEntity(guid);
            return Ok(response);
        }
    }
}
