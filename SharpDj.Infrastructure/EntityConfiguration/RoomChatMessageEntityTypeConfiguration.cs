using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharpDj.Domain.Entity;

namespace SharpDj.Infrastructure.EntityConfiguration
{
    public class RoomChatMessageEntityTypeConfiguration : IEntityTypeConfiguration<RoomChatMessageEntity>
    {
        public void Configure(EntityTypeBuilder<RoomChatMessageEntity> builder)
        {
            builder.HasOne(x => x.User)
                .WithMany(x => x.Posts)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder.HasOne(x => x.Room)
                .WithMany(x => x.Posts)
                .HasForeignKey(x=>x.RoomId)
                .IsRequired();

            builder.Property(x => x.Text)
                .HasMaxLength(512);


            builder.HasIndex(x => x.Text);
        }
    }
}
