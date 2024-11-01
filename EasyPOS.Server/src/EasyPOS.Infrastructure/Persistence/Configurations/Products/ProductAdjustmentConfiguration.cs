using EasyPOS.Domain.Products;

namespace EasyPOS.Infrastructure.Persistence.Configurations.Products;

public class ProductAdjustmentConfiguration : IEntityTypeConfiguration<ProductAdjustment>
{
    public void Configure(EntityTypeBuilder<ProductAdjustment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();

        builder.Property(t => t.AttachmentUrl)
            .HasMaxLength(250)
            .IsRequired(false);

        builder.Property(t => t.Note)
               .HasMaxLength(500)
               .IsRequired();

        builder.Property(t => t.ReferenceNo)
            .HasMaxLength(250)
            .IsRequired(false);

        builder.Property(x => x.TotalQuantity)
       .HasColumnType("decimal(10, 2)")
       .IsRequired();

        //builder.Ignore(e => e.DomainEvents);
    }
}


