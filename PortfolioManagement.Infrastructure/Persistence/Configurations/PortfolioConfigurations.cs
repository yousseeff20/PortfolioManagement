using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioManagement.Domain.Entities;
using PortfolioManagement.Domain.Enums;

namespace PortfolioManagement.Infrastructure.Persistence.Configurations;

public sealed class AboutMeConfiguration : IEntityTypeConfiguration<AboutMe>
{
    public void Configure(EntityTypeBuilder<AboutMe> builder)
    {
        builder.Property(x => x.FullName).HasMaxLength(160).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(180).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(180).IsRequired();
        builder.Property(x => x.Location).HasMaxLength(180).IsRequired();
        builder.Property(x => x.Bio).HasMaxLength(4000).IsRequired();
    }
}

public sealed class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Category).HasMaxLength(120).IsRequired();
        builder.HasIndex(x => new { x.Category, x.DisplayOrder });
    }
}

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.Property(x => x.Title).HasMaxLength(180).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(4000).IsRequired();
        var stringListComparer = new ValueComparer<List<string>>(
            (left, right) => left != null && right != null && left.SequenceEqual(right),
            value => value.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
            value => value.ToList());

        builder.Property(x => x.Technologies).HasConversion(
            v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
            v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .Metadata.SetValueComparer(stringListComparer);
        builder.Property(x => x.GalleryImageUrls).HasConversion(
            v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
            v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .Metadata.SetValueComparer(stringListComparer);
        builder.HasIndex(x => x.FeaturedProject);
    }
}

public sealed class ExperienceConfiguration : IEntityTypeConfiguration<Experience>
{
    public void Configure(EntityTypeBuilder<Experience> builder)
    {
        builder.Property(x => x.Company).HasMaxLength(180).IsRequired();
        builder.Property(x => x.Position).HasMaxLength(180).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(3000).IsRequired();
    }
}

public sealed class EducationConfiguration : IEntityTypeConfiguration<Education>
{
    public void Configure(EntityTypeBuilder<Education> builder)
    {
        builder.Property(x => x.Institution).HasMaxLength(180).IsRequired();
        builder.Property(x => x.Degree).HasMaxLength(180).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(3000).IsRequired();
    }
}

public sealed class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.Property(x => x.Title).HasMaxLength(180).IsRequired();
        builder.Property(x => x.Issuer).HasMaxLength(180).IsRequired();
    }
}

public sealed class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
{
    public void Configure(EntityTypeBuilder<ContactMessage> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(180).IsRequired();
        builder.Property(x => x.Subject).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Message).HasMaxLength(4000).IsRequired();
        builder.HasIndex(x => x.IsRead);
    }
}

public sealed class TestimonialConfiguration : IEntityTypeConfiguration<Testimonial>
{
    public void Configure(EntityTypeBuilder<Testimonial> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
        builder.Property(x => x.JobTitle).HasMaxLength(160).IsRequired();
        builder.Property(x => x.Comment).HasMaxLength(1500).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32).HasDefaultValue(TestimonialStatus.Pending);
        builder.HasIndex(x => x.Status);
    }
}

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.Property(x => x.TokenHash).HasMaxLength(128).IsRequired();
        builder.HasIndex(x => x.TokenHash).IsUnique();
        builder.HasOne(x => x.User).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserId);
    }
}
