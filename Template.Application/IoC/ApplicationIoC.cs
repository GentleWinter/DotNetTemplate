using Microsoft.Extensions.DependencyInjection;
using Template.Application.Helper;
using Template.Application.Services;

namespace Template.Application.IoC;

public static class ApplicationIoC
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AuthCodeStoreService>();
        services.AddScoped<ClientService>();
        services.AddScoped<LoginService>();
        services.AddScoped<StripeService>();
        services.AddScoped<TokenService>();
        
        services.AddScoped<PlanPriceHelper>();
        
        return services;
    }
}