using EasyPOS.Domain.Quotations;

namespace EasyPOS.Infrastructure.Persistence.Configurations.Quotations;

internal sealed class QuotationConfiguration : IEntityTypeConfiguration<Quotation>
{
    public void Configure(EntityTypeBuilder<Quotation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();

        builder.Property(x => x.QuotationDate)
               .IsRequired();

        builder.Property(x => x.ReferenceNo)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.WarehouseId)
               .IsRequired();

        builder.Property(x => x.CustomerId)
               .IsRequired();

        builder.Property(x => x.QuotationStatusId)
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

        builder.Property(x => x.QuotationNote)
               .HasMaxLength(500);

        builder.HasMany(x => x.QuotationDetails)
               .WithOne(x => x.Quotation)
               .HasForeignKey(x => x.QuotationId);
    }
}


