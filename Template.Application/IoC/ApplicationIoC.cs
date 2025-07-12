using Microsoft.Extensions.DependencyInjection;
using Template.Application.Services;

namespace Template.Application.IoC;

public static class ApplicationIoC
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ClientService>();
        services.AddScoped<TokenService>();
        
        return services;
    }
}