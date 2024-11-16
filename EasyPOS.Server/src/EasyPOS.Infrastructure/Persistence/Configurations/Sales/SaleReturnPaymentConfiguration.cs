using EasyPOS.Domain.Sales;

namespace EasyPOS.Infrastructure.Persistence.Configurations.Sales;

internal sealed class SaleReturnPaymentConfiguration : IEntityTypeConfiguration<SaleReturnPayment>
{
    public void Configure(EntityTypeBuilder<SaleReturnPayment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();

        builder.Property(x => x.SaleReturnId)
               .IsRequired();

        builder.Property(x => x.PaymentDate)
               .IsRequired();

        builder.Property(x => x.ReceivedAmount)
               .HasColumnType("decimal(18, 2)")
               .IsRequired();

        builder.Property(x => x.PayingAmount)
               .HasColumnType("decimal(18, 2)")
               .IsRequired();

        builder.Property(x => x.ChangeAmount)
               .HasColumnType("decimal(18, 2)");

        builder.Property(x => x.PaymentType)
               .IsRequired();

        builder.Property(x => x.Note)
               .HasMaxLength(500)
               .IsRequired(false);

        builder.HasOne(x => x.SaleReturn)
               .WithMany(x => x.SaleReturnPayments) // If SaleReturn has no navigation property for SaleReturnPayments
               .HasForeignKey(x => x.SaleReturnId);
    }
}
