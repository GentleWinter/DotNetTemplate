using Microsoft.EntityFrameworkCore;
using Template.Domain.Entities;

namespace Template.Infra.Contexts;

public class TemplateContext(DbContextOptions<TemplateContext> options) : DbContext(options)
{
    public DbSet<Client> Client { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.EnableDetailedErrors();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TemplateContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}