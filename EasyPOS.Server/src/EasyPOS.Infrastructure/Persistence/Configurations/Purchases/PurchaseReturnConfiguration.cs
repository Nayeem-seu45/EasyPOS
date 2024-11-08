using EasyPOS.Domain.Purchases;

namespace EasyPOS.Infrastructure.Persistence.Configurations.Purchases;

internal sealed class PurchaseReturnConfiguration : IEntityTypeConfiguration<PurchaseReturn>
{
    public void Configure(EntityTypeBuilder<PurchaseReturn> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();

        builder.Property(x => x.ReturnDate)
               .IsRequired();

        builder.Property(x => x.ReferenceNo)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.WarehouseId)
               .IsRequired();

        builder.Property(x => x.SupplierId)
               .IsRequired();

        builder.Property(x => x.ReturnStatusId)
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

        builder.HasOne(x => x.Purchase)
           .WithMany(x => x.PurchaseReturns)
           .HasForeignKey(x => x.PurchaseId)
           .OnDelete(DeleteBehavior.ClientSetNull);
    }
}


