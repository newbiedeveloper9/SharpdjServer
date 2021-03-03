using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharpDj.Domain.Entity;

namespace SharpDj.Infrastructure.EntityConfiguration
{
    public class UserAuthEntityTypeConfiguration : IEntityTypeConfiguration<UserAuthEntity>
    {
        public void Configure(EntityTypeBuilder<UserAuthEntity> builder)
        {
            builder.HasOne(x => x.User)
                .WithOne(x => x.UserAuthEntity)
                .HasForeignKey<UserAuthEntity>(x=>x.UserId)
                .IsRequired();

            builder.Property(x => x.Login)
                .HasMaxLength(32);

            builder.Property(x => x.AuthenticationKey)
                .HasMaxLength(128);

            builder.Property(x => x.Hash)
                .HasMaxLength(512);

            builder.Property(x => x.Salt)
                .HasMaxLength(32);


            builder.HasIndex(x => x.Login);
        }
    }
}
