using EasyPOS.Domain.Products;

namespace EasyPOS.Infrastructure.Persistence.Configurations.Products;

internal sealed class ProductAdjustmentDetailConfiguration : IEntityTypeConfiguration<ProductAdjustmentDetail>
{
    public void Configure(EntityTypeBuilder<ProductAdjustmentDetail> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();

        builder.Property(x => x.ProductAdjustmentId)
               .IsRequired();

        builder.Property(x => x.ProductId)
               .IsRequired();

        builder.Property(x => x.ProductCode)
               .HasMaxLength(50)
               .IsRequired(false);

        builder.Property(x => x.ProductName)
               .HasMaxLength(250)
               .IsRequired(false);

        builder.Property(x => x.UnitCost)
               .HasColumnType("decimal(18, 2)")
               .IsRequired();

        builder.Property(x => x.Quantity)
               .HasColumnType("decimal(10, 2)")
               .IsRequired();

        builder.Property(x => x.CurrentStock)
               .HasColumnType("decimal(10, 2)")
               .IsRequired();

        builder.HasOne(x => x.ProductAdjustment)
            .WithMany(x => x.ProductAdjustmentDetails)
            .HasForeignKey(x => x.ProductAdjustmentId)
            .OnDelete(DeleteBehavior.NoAction);

    }
}
