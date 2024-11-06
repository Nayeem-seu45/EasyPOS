using EasyPOS.Domain.Sales;

namespace EasyPOS.Infrastructure.Persistence.Configurations.Sales;

public class GiftCardConfiguration : IEntityTypeConfiguration<GiftCard>
{
    public void Configure(EntityTypeBuilder<GiftCard> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();

        builder.Property(t => t.CardNo)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(t => t.Amount)
            .HasColumnType("decimal(18, 2)")
            .IsRequired();

        builder.Property(t => t.Expense)
            .HasColumnType("decimal(18, 2)")
            .IsRequired();

        builder.Property(t => t.Balance)
            .HasColumnType("decimal(18, 2)")
            .IsRequired();

        //builder.Ignore(e => e.DomainEvents);
    }
}


