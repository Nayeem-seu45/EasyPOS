using EasyPOS.Domain.Trades;

namespace EasyPOS.Infrastructure.Persistence.Configurations.Trades;

public class CourierConfiguration : IEntityTypeConfiguration<Courier>
{
    public void Configure(EntityTypeBuilder<Courier> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();


        builder.Property(t => t.Name)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(t => t.PhoneNo)
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(t => t.MobileNo)
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(t => t.Email)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(t => t.Address)
            .HasMaxLength(250)
            .IsRequired();

        //builder.Ignore(e => e.DomainEvents);
    }
}


