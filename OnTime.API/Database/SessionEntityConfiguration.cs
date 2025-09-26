using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using OnTime.API.Models.Domain;

namespace OnTime.API.Database
{
    public class SessionEntityConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(s => s.Description)
                .HasMaxLength(1024);

            builder.Property(s => s.DurationInMinutes)
                .IsRequired();

            builder.HasOne(s => s.Organization)
                            .WithMany(o => o.Sessions)
                            .HasForeignKey(s => s.OrganizationId)
                            .IsRequired();

            builder.Property(s => s.CreatedAt)
                .IsRequired()
                .HasDefaultValue(DateTime.Now);
        }
    }
}
