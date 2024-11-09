using EasyPOS.Domain.Sales;

namespace EasyPOS.Infrastructure.Persistence.Configurations.Sales;

internal sealed class SaleReturnConfiguration : IEntityTypeConfiguration<SaleReturn>
{
    public void Configure(EntityTypeBuilder<SaleReturn> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();

        builder.Property(x => x.ReturnDate)
               .IsRequired();

        builder.Property(x => x.SoldReferenceNo)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.ReferenceNo)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.WarehouseId)
               .IsRequired();

        builder.Property(x => x.CustomerId)
               .IsRequired();

        builder.Property(x => x.ReturnStatusId)
               .IsRequired();

        builder.Property(x => x.AttachmentUrl)
               .HasMaxLength(255);

        builder.Property(x => x.SubTotal)
               .HasColumnType("decimal(18, 2)")
               .IsRequired();

        builder.Property(x => x.TaxRate)
               .HasColumnType("decimal(18, 2)");

        builder.Property(x => x.TaxAmount)
               .HasColumnType("decimal(18, 2)");

        builder.Property(x => x.DiscountRate)
              .HasColumnType("decimal(18, 2)");

        builder.Property(x => x.DiscountAmount)
           .HasColumnType("decimal(18, 2)");

        builder.Property(x => x.ShippingCost)
               .HasColumnType("decimal(18, 2)");

        builder.Property(x => x.GrandTotal)
              .HasColumnType("decimal(18, 2)")
              .IsRequired();

        builder.Property(x => x.PaidAmount)
              .HasColumnType("decimal(18, 2)")
              .HasDefaultValue(0)
              .IsRequired();

        builder.Property(x => x.DueAmount)
              .HasColumnType("decimal(18, 2)")
              .HasDefaultValue(0)
              .IsRequired();

        builder.Property(x => x.ReturnNote)
               .HasMaxLength(500);

        builder.Property(x => x.StaffNote)
               .HasMaxLength(500);

        builder.HasOne(x => x.Sale)
               .WithMany(x => x.SaleReturns)
               .HasForeignKey(x => x.SaleId)
               .OnDelete(DeleteBehavior.ClientSetNull);

    }
}


