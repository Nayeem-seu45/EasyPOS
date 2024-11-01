using EasyPOS.Domain.Products;

namespace EasyPOS.Infrastructure.Persistence.Configurations.Products;

public class CountStockBrandConfiguration : IEntityTypeConfiguration<CountStockBrand>
{
    public void Configure(EntityTypeBuilder<CountStockBrand> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();

        builder.HasOne(x => x.CountStock)
               .WithMany(x => x.CountStockBrands)
               .HasForeignKey(x => x.CountStockId)
               .OnDelete(DeleteBehavior.NoAction);


        //builder.Ignore(e => e.DomainEvents);
    }
}


