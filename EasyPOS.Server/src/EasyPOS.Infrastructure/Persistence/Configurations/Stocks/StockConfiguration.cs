using EasyPOS.Domain.Stocks;

namespace EasyPOS.Infrastructure.Persistence.Configurations.Stocks;

internal sealed class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()")
            .ValueGeneratedOnAdd();

        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.WarehouseId);

        builder.ToTable(x => x.HasCheckConstraint("CK_Stock_Quantity", "[Quantity] >= 0"));
        builder.ToTable(x => x.HasCheckConstraint("CK_Stock_UnitCost", "[UnitCost] >= 0"));

        builder.Property(x => x.Quantity)
            .HasColumnType("decimal(18, 2)")
            .HasDefaultValue(0)
            .IsRequired();


        builder.Property(x => x.UnitCost)
            .HasColumnType("decimal(18, 2)")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.AverageUnitCost)
            .HasColumnType("decimal(18, 2)")
            .HasDefaultValue(0)
            .IsRequired();

        builder.HasOne(x => x.Product)
            .WithMany(x => x.Stocks)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne(x => x.Warehouse)
            .WithMany(x => x.Stocks)
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.ClientSetNull);


    }
}
