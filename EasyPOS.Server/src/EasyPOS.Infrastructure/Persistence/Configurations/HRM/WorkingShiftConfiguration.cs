using EasyPOS.Domain.HRM;

namespace EasyPOS.Infrastructure.Persistence.Configurations.HRM;

public class WorkingShiftConfiguration : IEntityTypeConfiguration<WorkingShift>
{
    public void Configure(EntityTypeBuilder<WorkingShift> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();


        builder.Property(x => x.ShiftName)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(250)
            .IsRequired(false);

        //builder.Ignore(e => e.DomainEvents);
    }
}


