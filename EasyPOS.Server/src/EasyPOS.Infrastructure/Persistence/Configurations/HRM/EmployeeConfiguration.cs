using EasyPOS.Domain.HRM;

namespace EasyPOS.Infrastructure.Persistence.Configurations.HRM;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();

        builder.Property(x => x.Code)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.FirstName)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(250)
            .HasDefaultValue("")
            .IsRequired(false);

        builder.Property(x => x.Gender)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.NID)
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(x => x.Email)
            .HasMaxLength(250)
            .IsRequired(false);

        builder.Property(x => x.PhoneNo)
            .HasMaxLength(250)
            .IsRequired(false);

        builder.Property(x => x.MobileNo)
            .HasMaxLength(250)
            .IsRequired(false);

        builder.Property(x => x.Country)
            .HasMaxLength(250)
            .IsRequired(false);

        builder.Property(x => x.City)
            .HasMaxLength(250)
            .IsRequired(false);

        builder.Property(x => x.Address)
            .HasMaxLength(250)
            .IsRequired(false);

        //builder.Ignore(e => e.DomainEvents);
    }
}


