using EasyPOS.Domain.HRM;

namespace EasyPOS.Infrastructure.Persistence.Configurations.HRM;

public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();


        builder.Property(x => x.AttachmentUrl)
            .HasMaxLength(250)
            .IsRequired(false);

        builder.Property(x => x.Reason)
            .HasMaxLength(250)
            .IsRequired(false);

        //builder.Ignore(e => e.DomainEvents);
    }
}


