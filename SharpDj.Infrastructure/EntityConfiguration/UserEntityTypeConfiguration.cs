using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharpDj.Domain.Entity;

namespace SharpDj.Infrastructure.EntityConfiguration
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasOne(x => x.UserAuthEntity)
                .WithOne(x => x.User)
                .HasForeignKey<UserAuthEntity>(x => x.UserId)
                .IsRequired();

            builder.HasMany(x => x.Rooms)
                .WithOne(x => x.Author)
                .HasForeignKey(x => x.AuthorId)
                .IsRequired();

            builder.HasMany(x => x.UserAudits)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            builder.HasMany(x => x.Posts)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Username)
                .HasMaxLength(32);

            builder.Property(x => x.Email)
                .HasMaxLength(80);


            builder.HasIndex(x => x.Username);
            builder.HasIndex(x => x.Email);
        }
    }
}
