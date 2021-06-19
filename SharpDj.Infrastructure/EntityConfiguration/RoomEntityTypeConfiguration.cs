using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharpDj.Domain.Entity;

namespace SharpDj.Infrastructure.EntityConfiguration
{
    public class RoomEntityTypeConfiguration : IEntityTypeConfiguration<RoomEntity>
    {
        public void Configure(EntityTypeBuilder<RoomEntity> builder)
        {
            builder.HasOne(x => x.Author)
                .WithMany(x => x.Rooms)
                .HasForeignKey(x=>x.AuthorId)
                .IsRequired();

            builder.HasOne(x => x.Config)
                .WithOne(x => x.Room)
                .HasForeignKey<RoomConfigEntity>(x => x.RoomId)
                .IsRequired();

            builder.HasMany(x => x.Posts)
                .WithOne(x => x.Room)
                .HasForeignKey(x => x.RoomId)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(64);

            builder.Property(x => x.ImagePath)
                .HasMaxLength(200);


            builder.HasIndex(x => x.Name);
        }
    }
}
