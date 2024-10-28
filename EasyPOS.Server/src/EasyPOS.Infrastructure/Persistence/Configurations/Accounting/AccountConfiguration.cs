using EasyPOS.Domain.Accounting;

namespace EasyPOS.Infrastructure.Persistence.Configurations.Accounting;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();

        builder.Property(t => t.Balance)
            .HasColumnType("decimal(18, 2)");

        builder.Property(t => t.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Note)
            .HasMaxLength(200)
            .IsRequired(false);
        //builder.Ignore(e => e.DomainEvents);
    }
}


