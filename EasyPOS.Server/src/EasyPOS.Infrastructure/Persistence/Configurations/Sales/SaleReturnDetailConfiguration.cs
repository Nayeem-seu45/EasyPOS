﻿using EasyPOS.Domain.Sales;

namespace EasyPOS.Infrastructure.Persistence.Configurations.Sales;

internal sealed class SaleReturnDetailConfiguration : IEntityTypeConfiguration<SaleReturnDetail>
{
    public void Configure(EntityTypeBuilder<SaleReturnDetail> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();

        builder.Property(x => x.SaleReturnId)
               .IsRequired();

        builder.Property(x => x.ProductId)
               .IsRequired();

        builder.Property(x => x.SoldQuantity)
               .IsRequired();

        builder.Property(x => x.ReturnedQuantity)
               .IsRequired();

        builder.Property(x => x.BatchNo)
               .HasMaxLength(50)
               .IsRequired(false);

        builder.Property(x => x.ProductCode)
               .HasMaxLength(50)
               .IsRequired(false);

        builder.Property(x => x.ProductName)
               .HasMaxLength(250)
               .IsRequired(false);

        builder.Property(x => x.ProductUnitCost)
               .HasColumnType("decimal(18, 2)")
               .IsRequired();

        builder.Property(x => x.ProductUnitPrice)
               .HasColumnType("decimal(18, 2)")
               .IsRequired();

        builder.Property(x => x.ProductUnitDiscount)
               .HasColumnType("decimal(18, 2)")
               .IsRequired();

        builder.Property(x => x.ProductUnit)
               .HasColumnType("decimal(10, 2)")
               .IsRequired();

        builder.Property(x => x.ExpiredDate);

        builder.Property(x => x.NetUnitPrice)
               .HasColumnType("decimal(18, 2)")
               .IsRequired();

        builder.Property(x => x.DiscountRate)
               .HasColumnType("decimal(4, 2)")
               .IsRequired();

        builder.Property(x => x.DiscountAmount)
               .HasColumnType("decimal(18, 2)")
               .IsRequired();

        builder.Property(x => x.TaxRate)
               .HasColumnType("decimal(4, 2)")
               .IsRequired();

        builder.Property(x => x.TaxAmount)
               .HasColumnType("decimal(18, 2)")
               .IsRequired();

        builder.Property(x => x.TotalPrice)
               .HasColumnType("decimal(18, 2)")
               .IsRequired();

        builder.Property(x => x.Remarks)
               .HasMaxLength(500)
               .IsRequired(false);


        builder.HasOne(x => x.SaleReturn)
               .WithMany(x => x.SaleReturnDetails)
               .HasForeignKey(x => x.SaleReturnId)
               .OnDelete(DeleteBehavior.ClientSetNull);
    }
}