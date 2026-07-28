using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GM.EntityFramework.Sample.Persistence.Configuration;

public class SampleConfiguration 
    : IEntityTypeConfiguration<Domain.BoundedContext.SampleBoundedContext.SampleAggregate.Sample>
{
    public void Configure(EntityTypeBuilder<Domain.BoundedContext.SampleBoundedContext.SampleAggregate.Sample> builder)
    {
        builder.ToTable("Sample", "sample");

        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Id);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.IsHidden)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasMany(e => e.SampleItems)
            .WithOne(ee => ee.Sample)
            .HasForeignKey(ee => ee.SampleId)
            .IsRequired();
    }
}