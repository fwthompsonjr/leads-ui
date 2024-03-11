using legallead.permissions.api.Entities;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly string endpointSecret;
        public EventsController(StripeKeyEntity config)
        {
            endpointSecret = config.WebhookId ?? string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            try
            {
                var stripeEvent = await GetEvent(endpointSecret, Request);
                if (stripeEvent == null) { return BadRequest(); }
                switch (stripeEvent.Type)
                {
                    case "checkout.session.completed":
                        // Payment is successful and the subscription is created.
                        // You should provision the subscription and save the customer ID to your database.
                        break;
                    case "invoice.paid":
                        // Continue to provision the subscription as payments continue to be made.
                        // Store the status in your database and check when a user accesses your service.
                        // This approach helps you avoid hitting rate limits.
                        break;
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

        private static async Task<Event?> GetEvent(string webhook, HttpRequest request)
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
