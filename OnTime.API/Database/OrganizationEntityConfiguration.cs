using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using OnTime.API.Models.Domain;

namespace OnTime.API.Database
{
    public class OrganizationEntityConfiguration : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.HasMany(o => o.Sessions)
                .WithOne(o => o.Organization)
                .HasForeignKey(s => s.OrganizationId);

            builder.HasMany(o => o.Professionals)
                .WithOne(o => o.Organization)
                .HasForeignKey(u => u.OrganizationId);

            builder.Property(o => o.CreatedAt)
                .IsRequired()
                .HasDefaultValue(DateTime.Now);
        }
    }
}
