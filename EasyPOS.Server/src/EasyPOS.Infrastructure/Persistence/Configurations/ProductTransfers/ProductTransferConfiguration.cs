using EasyPOS.Domain.ProductTransfers;

namespace EasyPOS.Infrastructure.Persistence.Configurations.ProductTransfers;

internal sealed class ProductTransferConfiguration : IEntityTypeConfiguration<ProductTransfer>
{
    public void Configure(EntityTypeBuilder<ProductTransfer> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();

        builder.Property(x => x.TransferDate)
               .IsRequired();

        builder.Property(x => x.ReferenceNo)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.FromWarehouseId)
               .IsRequired();

        builder.Property(x => x.ToWarehouseId)
               .IsRequired();

        builder.Property(x => x.TransferStatusId)
               .IsRequired();

        builder.Property(x => x.AttachmentUrl)
               .HasMaxLength(255);

        builder.Property(x => x.SubTotal)
               .HasColumnType("decimal(18, 2)")
               .IsRequired();

        builder.Property(x => x.TaxRate)
               .HasColumnType("decimal(4, 2)");

        builder.Property(x => x.TaxAmount)
               .HasColumnType("decimal(18, 2)");

        builder.Property(x => x.DiscountRate)
                .HasColumnType("decimal(3, 2)");

        builder.Property(x => x.DiscountAmount)
               .HasColumnType("decimal(18, 2)");

        builder.Property(x => x.ShippingCost)
               .HasColumnType("decimal(18, 2)");

        builder.Property(x => x.GrandTotal)
              .HasColumnType("decimal(18, 2)")
              .IsRequired();

        builder.Property(x => x.Note)
               .HasMaxLength(500);

    }
}


