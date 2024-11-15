using EasyPOS.Domain.Purchases;

namespace EasyPOS.Infrastructure.Persistence.Configurations.Purchases;

internal sealed class PurchaseReturnPaymentConfiguration : IEntityTypeConfiguration<PurchaseReturnPayment>
{
    public void Configure(EntityTypeBuilder<PurchaseReturnPayment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();

        builder.Property(x => x.PurchaseReturnId)
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
               .HasMaxLength(500);

        builder.HasOne(x => x.PurchaseReturn)
               .WithMany(x => x.PurchaseReturnPayments)
               .HasForeignKey(x => x.PurchaseReturnId);
    }
}
