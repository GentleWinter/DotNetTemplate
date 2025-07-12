using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;
using Template.Domain.Entities;
using Template.Application.Helper;
using Plan = Template.Domain.Entities.Plan;

namespace Template.Application.Services;

public class StripeService (IConfiguration _configuration, ILogger<StripeService> logger, PlanPriceHelper _planPriceHelper)
{
    public string CreateStripeCheckoutSession(string email, Plan plan)
    {
        var options = new SessionCreateOptions
        {
            CustomerEmail = email,
            PaymentMethodTypes = ["card"],
            Mode = "subscription",
            LineItems =
            [
                new SessionLineItemOptions
                {
                    Price = _planPriceHelper.ObtainPrice(plan),
                    Quantity = 1,
                }
            ],
            SuccessUrl = _configuration["ApplicationLinks:SuccessNewClient"],
            CancelUrl = _configuration["ApplicationLinks:FailureNewClient"],
            SubscriptionData = new SessionSubscriptionDataOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    { "clientEmail", email },
                    { "action", "CreateClient" }
                }
            }
        };

        var service = new SessionService();
        var session = service.Create(options);

        return session.Url;
    }
}