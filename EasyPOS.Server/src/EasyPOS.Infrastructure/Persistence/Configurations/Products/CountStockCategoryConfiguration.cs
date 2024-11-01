using EasyPOS.Domain.Products;

namespace EasyPOS.Infrastructure.Persistence.Configurations.Products;

public class CountStockCategoryConfiguration : IEntityTypeConfiguration<CountStockCategory>
{
    public void Configure(EntityTypeBuilder<CountStockCategory> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();


        builder.HasOne(x => x.CountStock)
                .WithMany(x => x.CountStockCategories)
                .HasForeignKey(x => x.CountStockId)
                .OnDelete(DeleteBehavior.NoAction);

        //builder.Ignore(e => e.DomainEvents);
    }
}


