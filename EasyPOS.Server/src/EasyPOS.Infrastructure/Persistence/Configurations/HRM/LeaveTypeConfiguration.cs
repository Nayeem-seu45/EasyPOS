using EasyPOS.Domain.HRM;

namespace EasyPOS.Infrastructure.Persistence.Configurations.HRM;

public class LeaveTypeConfiguration : IEntityTypeConfiguration<LeaveType>
{
    public void Configure(EntityTypeBuilder<LeaveType> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.Code)
            .HasMaxLength(10)
            .IsRequired(false);

        //builder.Ignore(e => e.DomainEvents);
    }
}


