using EasyPOS.Domain.ProductTransfers;

namespace EasyPOS.Infrastructure.Persistence.Configurations.ProductTransfers;

internal sealed class ProductTransferDetailConfiguration : IEntityTypeConfiguration<ProductTransferDetail>
{
    public void Configure(EntityTypeBuilder<ProductTransferDetail> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();

        builder.Property(x => x.ProductTransferId)
               .IsRequired();

        builder.Property(x => x.ProductId)
              .IsRequired();

        builder.Property(x => x.Quantity)
               .IsRequired();

        builder.Property(x => x.BatchNo)
               .HasMaxLength(50)
               .IsRequired(false);

        builder.Property(x => x.ProductCode)
               .HasMaxLength(50)
               .IsRequired(false);

        builder.Property(x => x.ProductName)
               .HasMaxLength(250)
               .IsRequired(false);

        builder.Property(x => x.ProductUnitCost)
               .HasColumnType("decimal(18, 2)")
               .IsRequired();

        builder.Property(x => x.ProductUnitPrice)
               .HasColumnType("decimal(18, 2)")
               .IsRequired();

        builder.Property(x => x.ProductUnitDiscount)
               .HasColumnType("decimal(18, 2)")
               .IsRequired();

        builder.Property(x => x.ProductUnit)
               .HasColumnType("decimal(10, 2)")
               .IsRequired();

        builder.Property(x => x.ExpiredDate);

        builder.Property(x => x.NetUnitCost)
               .HasColumnType("decimal(18, 2)")
               .IsRequired();

        builder.Property(x => x.DiscountRate)
               .HasColumnType("decimal(4, 2)")
               .IsRequired();

        builder.Property(x => x.DiscountAmount)
               .HasColumnType("decimal(18, 2)")
               .IsRequired();

        builder.Property(x => x.TaxRate)
               .HasColumnType("decimal(18, 2)")
               .IsRequired();

        builder.Property(x => x.TaxAmount)
               .HasColumnType("decimal(18, 2)")
               .IsRequired();

        builder.Property(x => x.TotalPrice)
               .HasColumnType("decimal(18, 2)")
               .IsRequired();

        builder.Property(x => x.Remarks)
               .HasMaxLength(500)
               .IsRequired(false);

        builder.HasOne(x => x.ProductTransfer)
               .WithMany(x => x.ProductTransferDetails)
               .HasForeignKey(x => x.ProductTransferId)
               .OnDelete(DeleteBehavior.NoAction);


    }
}
