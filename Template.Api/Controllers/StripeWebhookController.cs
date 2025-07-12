using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Template.Application.Services;

namespace Template.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class StripeWebhookController(ILogger<StripeWebhookController> logger, ClientService clientService)
: ControllerBase
{
        [HttpPost]
        public async Task<IActionResult> HandleStripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signatureHeader = Request.Headers["Stripe-Signature"];

            if (string.IsNullOrEmpty(signatureHeader))
            {
                return BadRequest("Missing Stripe-Signature header");
            }

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(json,
                                                          signatureHeader, 
                                                          Environment.GetEnvironmentVariable("WEBHOOK_SECRET")
                                                          ,throwOnApiVersionMismatch: false
                                                            );
            }
            catch (StripeException e)
            {
                logger.LogError($"⚠️ Stripe webhook error while parsing event: {e.Message}");
                return BadRequest();
            }

            try
            {
                var eventType = stripeEvent.Type;

                switch (eventType)
                {
                    case "checkout.session.completed":
                    {
                        var session = stripeEvent.Data.Object as Session;
                        if (session is null)
                            return BadRequest("session could not be null");
                        
                        logger.LogInformation("✅ Checkout completed for session: {sessionId}", session.Id);
                        if (session.PaymentStatus != "paid" || session.Status != "complete") return Ok();
                        
                        var ret =  clientService.ActivateClient(session.Metadata["Id"]);
                            
                        if (ret.IsFailed)
                            return BadRequest(ret.Errors);

                        break;

                    }
                    case "payment_intent.succeeded":
                    {
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        logger.LogInformation($"💰 PaymentIntent succeeded: {paymentIntent?.Id}");

                        if (paymentIntent is { Metadata: not null }
                            && paymentIntent.Metadata.ContainsKey("clientEmail")
                            && paymentIntent.Status == "succeeded")
                        {
                            var email = paymentIntent.Metadata["clientEmail"];

                            try
                             {
                                 clientService.ActivateClient(email);
                                 logger.LogInformation("✅ Client activated: {Email}", email);
                             }
                             catch (Exception ex)
                             {
                                 logger.LogError(ex, "❌ Error adding credits for client {Email} - {erroMessage}", email, ex.Message);
                                 
                                 return BadRequest(ex.Message);
                             }
                            
                        }
                        else
                        {
                            logger.LogWarning("⚠️ PaymentIntent succeeded but clientEmail metadata is missing or invalid.");
                        }

                        break;
                    }
                    case "invoice.paid":
                    {
                        var invoice = stripeEvent.Data.Object as Invoice;

                        var metadata = invoice?.Lines?.Data?.FirstOrDefault()?.Metadata;

                        if (metadata != null
                            && metadata.ContainsKey("clientEmail"))
                        {
                            var email = metadata["clientEmail"];

                            if (metadata.TryGetValue("action", out var actionValue)
                                && actionValue.Equals("CreateClient", StringComparison.OrdinalIgnoreCase))
                            {
                                try
                                {
                                    clientService.ActivateClient(email);
                                    logger.LogInformation("✅ Client activated from invoice: {Email}", email);
                                }
                                catch (Exception ex)
                                {
                                    logger.LogError(ex, "❌ Error activating client from invoice {Email} - {ErrorMessage}", email, ex.Message);
                                    return BadRequest(ex.Message);
                                }
                            }
                        }

                        break;
                    }
                    case "invoice.payment_succeeded":
                    {
                        var invoiceSucceeded = stripeEvent.Data.Object as Invoice;
                        logger.LogInformation($"✅ Invoice payment succeeded for customer: {invoiceSucceeded?.CustomerId}");
                        clientService.ActivateClient(invoiceSucceeded.CustomerEmail);
                        break;
                    }
                    case "invoice.payment_failed":
                    {
                        var invoiceFailed = stripeEvent.Data.Object as Invoice;
                        logger.LogInformation($"❌ Invoice payment failed for customer: {invoiceFailed?.CustomerId}");
                        clientService.InactivateClient(invoiceFailed.CustomerEmail);
                        break;
                    }
                    case "customer.subscription.deleted":
                    {
                        var subscriptionDeleted = stripeEvent.Data.Object as Subscription;
                        logger.LogInformation($"🚫 Subscription canceled: {subscriptionDeleted?.Id}");
                        break;
                    }
                    default:
                        logger.LogWarning("ℹ️ Unhandled event type: {eventType}", eventType);
                        break;
                }

                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError("Webhook handling error: {Message}", ex.Message);
                
                return StatusCode(500);
            }
        }
}