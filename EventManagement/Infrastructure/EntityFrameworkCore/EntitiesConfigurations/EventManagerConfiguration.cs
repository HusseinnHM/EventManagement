using EventManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManagement.Infrastructure.EntityFrameworkCore.EntitiesConfigurations;

public class EventManagerConfiguration : IEntityTypeConfiguration<EventManager>
{
    public void Configure(EntityTypeBuilder<EventManager> builder)
    {
        builder.Property(e => e.Name).HasColumnType("NVARCHAR").HasMaxLength(TypeMaxLengthConstants.NounString);
        builder.Property(e => e.Email).HasColumnType("NVARCHAR").HasMaxLength(TypeMaxLengthConstants.NounString);
        builder.Property(e => e.PasswordHash).HasColumnType("NVARCHAR").HasMaxLength(TypeMaxLengthConstants.SmallString);

        builder.HasIndex(e => e.Email).IsUnique();
    }
}