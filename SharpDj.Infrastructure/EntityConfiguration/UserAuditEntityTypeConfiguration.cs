using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharpDj.Domain.Entity;

namespace SharpDj.Infrastructure.EntityConfiguration
{
    public class UserAuditEntityTypeConfiguration : IEntityTypeConfiguration<UserAuditEntity>
    {
        public void Configure(EntityTypeBuilder<UserAuditEntity> builder)
        {
            builder.HasOne(x => x.User)
                .WithMany(x => x.UserAudits)
                .HasForeignKey(x=>x.UserId)
                .IsRequired();

            builder.Property(x => x.Ip)
                .HasMaxLength(15);


            builder.HasIndex(x => x.Date);
            builder.HasIndex(x => x.Ip);
        }
    }
}
