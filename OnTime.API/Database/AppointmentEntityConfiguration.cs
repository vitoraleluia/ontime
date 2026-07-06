using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using OnTime.API.Models.Domain;

namespace OnTime.API.Database
{
    public class AppointmentEntityConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.StartDate).IsRequired();
            builder.Property(a => a.EndDate).IsRequired();

            builder.HasMany(a => a.Services)
                .WithMany(s => s.Appointments);

            builder.HasOne(a => a.Professional)
                .WithMany()
                .HasForeignKey(a => a.ProfessionalId)
                .IsRequired();

            builder.HasOne(a => a.Client)
                .WithMany()
                .HasForeignKey(a => a.ClientId)
                .IsRequired();
        }
    }
}
