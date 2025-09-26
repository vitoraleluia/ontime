using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using OnTime.API.Models.Domain;

namespace OnTime.API.Database
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.IsProfessional)
                .IsRequired();

            builder.HasOne(u => u.Organization)
                .WithMany(o => o.Professionals)
                .HasForeignKey(u => u.OrganizationId);

            builder.HasMany(u => u.Appointments)
                .WithMany();
        }
    }
}
