using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sasquat.Infra.Alerts;
using Template.Infra.Contexts;
using Template.Infra.Data.Repository;
using Template.Infra.Data.Repository.Interface;

namespace Template.Infra.IoC;

public static class InfraIoC
{
    public static IServiceCollection AddInfra(this IServiceCollection services)
    {
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        
        services.AddDbContext<TemplateContext>
            (
                options => options.UseNpgsql(connectionString)
            );
        
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddScoped<DbContext, TemplateContext>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<EmailAlert>();
        
        return services;
    }
}