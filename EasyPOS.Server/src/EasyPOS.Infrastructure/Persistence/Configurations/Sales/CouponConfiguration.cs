using EasyPOS.Domain.Sales;

namespace EasyPOS.Infrastructure.Persistence.Configurations.Sales;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();


        builder.Property(t => t.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Name)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(t => t.Description)
            .HasMaxLength(250)
            .IsRequired(false);

        builder.Property(t => t.Amount)
            .HasColumnType("decimal(18, 2)")
            .IsRequired();

        builder.Property(t => t.MinimumSpend)
            .HasColumnType("decimal(18, 2)")
            .IsRequired(false);

        builder.Property(t => t.MaximumSpend)
            .HasColumnType("decimal(18, 2)")
            .IsRequired(false);

        builder.Property(t => t.PerCouponUsageLimit)
            .HasColumnType("decimal(18, 2)")
            .IsRequired(false);

        builder.Property(t => t.PerUserUsageLimit)
            .HasColumnType("decimal(18, 2)")
            .IsRequired(false);

        //builder.Ignore(e => e.DomainEvents);
    }
}


