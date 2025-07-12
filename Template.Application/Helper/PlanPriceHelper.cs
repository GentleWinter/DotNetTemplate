using Microsoft.Extensions.Configuration;
using Template.Domain.Entities;

namespace Template.Application.Helper;

public class PlanPriceHelper(IConfiguration configuration)
{
    private readonly string _standard = configuration["PlanPrices:Standard"] ?? "price_1RcdR0RmW0BIX27VYA7DQ06E";
    private readonly string _plus = configuration["PlanPrices:Plus"] ?? "price_1RcepuRmW0BIX27VzeiAHgMp";
    private readonly string _premium = configuration["PlanPrices:Premium"] ?? "price_1RcdS7RmW0BIX27VdGdsaMRA";
    
    public string ObtainPrice(Plan plan)
    {
        return plan switch
        {
            Plan.Standard => _standard,
            Plan.Plus => _plus,
            Plan.Premium => _premium,
            _ => throw new ArgumentOutOfRangeException(nameof(plan), plan, null)
        };
    }
}