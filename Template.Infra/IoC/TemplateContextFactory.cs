using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Template.Infra.Contexts;

namespace Template.Infra.IoC;

public class TemplateContextFactory : IDesignTimeDbContextFactory<TemplateContext>
{
    public TemplateContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TemplateContext>();

        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") 
                               ?? throw new ArgumentNullException("args");

        optionsBuilder.UseNpgsql(connectionString);
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        return new TemplateContext(optionsBuilder.Options);
    }
}