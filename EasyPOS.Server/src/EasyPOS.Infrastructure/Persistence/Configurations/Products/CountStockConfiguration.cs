using EasyPOS.Domain.Products;

namespace EasyPOS.Infrastructure.Persistence.Configurations.Products;

public class CountStockConfiguration : IEntityTypeConfiguration<CountStock>
{
    public void Configure(EntityTypeBuilder<CountStock> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();


        builder.Property(x => x.ReferenceNo)
               .HasMaxLength(50)
               .IsRequired();

        //builder.Ignore(e => e.DomainEvents);
    }
}


