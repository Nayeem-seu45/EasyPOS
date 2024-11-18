using EasyPOS.Domain.HRM;

namespace EasyPOS.Infrastructure.Persistence.Configurations.HRM;

public class WorkingShiftDetailConfiguration : IEntityTypeConfiguration<WorkingShiftDetail>
{
    public void Configure(EntityTypeBuilder<WorkingShiftDetail> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();

        builder.Property(x => x.StartTime)
              .HasColumnType("time") 
              .IsRequired(false);

        builder.Property(x => x.EndTime)
               .HasColumnType("time")
               .IsRequired(false);

        builder.Property(x => x.DayOfWeek)
               .IsRequired();

        builder.Property(x => x.IsWeekend)
               .IsRequired();

        // Foreign Key Relationship with WorkingShift
        builder.HasOne(x => x.WorkingShift)
               .WithMany(x => x.WorkingShiftDetails)
               .HasForeignKey(x => x.WorkingShiftId)
               .OnDelete(DeleteBehavior.ClientSetNull);

    }
}


