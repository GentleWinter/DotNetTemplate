using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Domain.Entities;

namespace Template.Infra.Mapper;


internal class ClientMapper : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Client")
            .HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("Id");

        builder.Property(x => x.Name)
            .HasColumnName("Name").IsRequired();

        builder.Property(x => x.Role)
            .HasColumnName("Role")
            .HasDefaultValue(Roles.Client);

        builder.Property(x => x.Plan)
            .HasColumnName("Plan");
        
        builder.Property(x => x.AuthInfo)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<AuthInfo>(v, (JsonSerializerOptions)null))
            .HasColumnName("AuthInfos")
            .IsRequired();

        builder.HasIndex(x => x.AuthInfo.Email, "Index_AuthEmail")
            .IsUnique();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(false)
            .HasColumnName("IsActive").IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("CreatedAt").IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("UpdatedAt");
    }
}