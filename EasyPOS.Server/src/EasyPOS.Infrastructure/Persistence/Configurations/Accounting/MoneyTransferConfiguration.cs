using EasyPOS.Domain.Accounting;

namespace EasyPOS.Infrastructure.Persistence.Configurations.Accounting;

public class MoneyTransferConfiguration : IEntityTypeConfiguration<MoneyTransfer>
{
    public void Configure(EntityTypeBuilder<MoneyTransfer> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();

        builder.Property(x => x.Amount)
            .HasColumnType("decimal(18, 2)");

        builder.Property(x => x.ReferenceNo)
            .HasMaxLength(200)
            .IsRequired(false);

        //builder.Ignore(e => e.DomainEvents);
    }
}


