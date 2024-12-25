using legallead.permissions.api.Entities;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ExcludeFromCodeCoverage(Justification = "This controller and component are not used. Future development.")]
    public class EventsController(
        StripeKeyEntity config,
        ILeadInvoiceService svcs) : ControllerBase
    {
        private readonly string endpointSecret = config.WebhookId ?? string.Empty;
        private readonly ILeadInvoiceService db = svcs;
        [HttpPost]
        public async Task<IActionResult> IndexAsync()
        {
            try
            {
                var stripeEvent = await GetEventAsync(endpointSecret, Request);
                if (stripeEvent == null) { return BadRequest(); }
                switch (stripeEvent.Type)
                {
                    case "checkout.session.completed":
                        // Payment is successful and the subscription is created.
                        // You should provision the subscription and save the customer ID to your database.
                        break;
                    case "invoice.paid":
                        if (stripeEvent.Data.Object is not Invoice invc) break;
                        var hasIndex = invc.Metadata.TryGetValue("RecordIndex", out var invoiceId);
                        if (!hasIndex || string.IsNullOrEmpty(invoiceId)) break;
                        var updated = db.CloseInvoiceAsync(invoiceId);
                        return Ok(updated);
                    case "invoice.payment_failed":
                        // The payment failed or the customer does not have a valid payment method.
                        // The subscription becomes past_due. Notify your customer and send them to the
                        // customer portal to update their payment information.
                        break;
                    default:
                        // Unhandled event type
                        Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                        break;
                }

                return Ok();
            }
            catch (StripeException e)
            {
                Console.WriteLine(e.Message);
                return BadRequest();
            }
        }

        private static async Task<Event?> GetEventAsync(string webhook, HttpRequest request)
        {
            try
            {
                var json = await new StreamReader(request.Body).ReadToEndAsync();
                var stripeEvent = EventUtility.ConstructEvent(
                  json,
                  request.Headers["Stripe-Signature"],
                  webhook
                );
                Console.WriteLine($"Webhook notification with type: {stripeEvent.Type} found for {stripeEvent.Id}");
                return stripeEvent;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something failed {e}");
                return null;
            }
        }
    }
}
