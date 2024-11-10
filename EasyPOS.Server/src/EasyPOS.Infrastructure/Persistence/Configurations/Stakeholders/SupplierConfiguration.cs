﻿using EasyPOS.Domain.Stakeholders;

namespace EasyPOS.Infrastructure.Persistence.Configurations.Products;

internal sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()")
                .ValueGeneratedOnAdd();

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

        builder.Property(p => p.Country)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(p => p.City)
           .HasMaxLength(200)
           .IsRequired(false);

        // Decimal precision for financial fields
        builder.Property(s => s.OpeningBalance)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(s => s.TotalDueAmount)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(s => s.PaidAmount)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(s => s.TotalAdvanceAmount)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(s => s.PreviousBalance)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(s => s.CreditLimit)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);
    }
}
