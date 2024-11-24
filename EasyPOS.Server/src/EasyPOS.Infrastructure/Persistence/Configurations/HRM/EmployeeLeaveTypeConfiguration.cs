using EasyPOS.Domain.HRM;

namespace EasyPOS.Infrastructure.Persistence.Configurations.HRM;

public class EmployeeLeaveTypeConfiguration : IEntityTypeConfiguration<EmployeeLeaveType>
{
    public void Configure(EntityTypeBuilder<EmployeeLeaveType> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();

    }
}
