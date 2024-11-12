using EasyPOS.Domain.Stakeholders;

namespace EasyPOS.Infrastructure.Persistence.Configurations.Products;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()")
                .ValueGeneratedOnAdd();

        builder.Property(p => p.IdentityNo)
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(p => p.Name)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(p => p.Address)
            .HasMaxLength(maxLength: 500)
            .IsRequired(false);

        builder.Property(p => p.Email)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(p => p.PhoneNo)
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(p => p.Mobile)
           .HasMaxLength(20)
           .IsRequired(false);

        builder.Property(p => p.TaxNumber)
            .HasMaxLength(250)
            .IsRequired(false);


        builder.Property(p => p.Country)
           .HasMaxLength(100)
           .IsRequired(false);

        builder.Property(p => p.City)
           .HasMaxLength(200)
           .IsRequired(false);

        builder.Property(t => t.TotalDueAmount)
            .HasColumnType("decimal(18, 2)")
            .HasDefaultValue(0);

        builder.Property(t => t.TotalPaidAmount)
            .HasColumnType("decimal(18, 2)")
            .HasDefaultValue(0);


        //builder.Property(t => t.OutstandingBalance)
        //    .HasColumnType("decimal(18, 2)")
        //    .HasDefaultValue(0);

        builder.Property(t => t.CreditLimit)
            .HasColumnType("decimal(18, 2)")
            .IsRequired(false);
    }
}
