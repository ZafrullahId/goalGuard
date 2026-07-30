using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using goalGuard.Entity;

namespace goalGuard.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Email)
               .IsUnique();

        builder.HasIndex(x => x.BmoniUserId)
               .IsUnique();

        builder.Property(x => x.OnboardingStatus)
               .HasConversion<string>();
    }
}
