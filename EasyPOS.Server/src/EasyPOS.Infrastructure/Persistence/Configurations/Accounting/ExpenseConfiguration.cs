using EasyPOS.Domain.Accounting;

namespace EasyPOS.Infrastructure.Persistence.Configurations.Accounting;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();

        builder.Property(t => t.Title)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(t => t.ReferenceNo)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(t => t.Amount)
            .HasColumnType("decimal(18, 2)")
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(t => t.AttachmentUrl)
            .HasMaxLength(250)
            .IsRequired(false);

        //builder.Ignore(e => e.DomainEvents);
    }
}


