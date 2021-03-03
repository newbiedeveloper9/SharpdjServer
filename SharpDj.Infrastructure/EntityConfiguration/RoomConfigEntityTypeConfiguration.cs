using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharpDj.Domain.Entity;

namespace SharpDj.Infrastructure.EntityConfiguration
{
    public class RoomConfigEntityTypeConfiguration : IEntityTypeConfiguration<RoomConfigEntity>
    {
        public void Configure(EntityTypeBuilder<RoomConfigEntity> builder)
        {
            builder.HasOne(x => x.Room)
                .WithOne(x => x.Config)
                .HasForeignKey<RoomConfigEntity>(x => x.RoomId)
                .IsRequired();

            builder.Property(x => x.LocalEnterMessage)
                .HasMaxLength(512);

            builder.Property(x => x.LocalLeaveMessage)
                .HasMaxLength(512);

            builder.Property(x => x.PublicEnterMessage)
                .HasMaxLength(512);

            builder.Property(x => x.PublicLeaveMessage)
                .HasMaxLength(512);
        }
    }
}
