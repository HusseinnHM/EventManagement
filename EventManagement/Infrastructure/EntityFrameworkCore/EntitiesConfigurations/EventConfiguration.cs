using EventManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManagement.Infrastructure.EntityFrameworkCore.EntitiesConfigurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.Property(e => e.Name).HasColumnType("NVARCHAR").HasMaxLength(TypeMaxLengthConstants.MediumString);
        builder.Property(e => e.Location).HasColumnType("NVARCHAR").HasMaxLength(TypeMaxLengthConstants.LongString);
        builder.Property(e => e.Description).HasColumnType("NVARCHAR").HasMaxLength(TypeMaxLengthConstants.MaxString);
        
        builder.HasIndex(e => e.StartDate);
        builder.Property(e => e.ConcurrencyStamp).IsConcurrencyToken();
    }
}