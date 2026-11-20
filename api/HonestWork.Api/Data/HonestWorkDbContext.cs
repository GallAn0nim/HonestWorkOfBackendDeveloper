using HonestWork.Api.Features.Auth;
using HonestWork.Api.Features.Cv;
using HonestWork.Api.Features.Quotes;
using Microsoft.EntityFrameworkCore;

namespace HonestWork.Api.Data;

public class HonestWorkDbContext(DbContextOptions<HonestWorkDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<CvProfile> CvProfiles => Set<CvProfile>();
    public DbSet<CvSkill> CvSkills => Set<CvSkill>();
    public DbSet<CvExperience> CvExperiences => Set<CvExperience>();
    public DbSet<CvEducation> CvEducations => Set<CvEducation>();
    public DbSet<CvLanguage> CvLanguages => Set<CvLanguage>();
    public DbSet<Quote> Quotes => Set<Quote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("citext");

        modelBuilder.Entity<User>(user =>
        {
            user.Property(candidate => candidate.Username).HasColumnType("citext").HasMaxLength(50);
            user.Property(candidate => candidate.CreatedAt).HasDefaultValueSql("now()");
            user.HasIndex(candidate => candidate.Username).IsUnique();
        });

        modelBuilder.Entity<CvProfile>(profile =>
        {
            profile.Property(candidate => candidate.LanguageCode).HasMaxLength(10);
            profile.Property(candidate => candidate.FullName).HasMaxLength(100);
            profile.Property(candidate => candidate.Title).HasMaxLength(100);
            profile.Property(candidate => candidate.Email).HasMaxLength(254);
            profile.Property(candidate => candidate.WebsiteUrl).HasMaxLength(300);
            profile.Property(candidate => candidate.WorkPreference).HasMaxLength(100);
            profile.HasIndex(candidate => candidate.LanguageCode).IsUnique();
            profile.HasMany(candidate => candidate.Skills).WithOne().HasForeignKey(skill => skill.ProfileId);
            profile.HasMany(candidate => candidate.Experiences).WithOne().HasForeignKey(experience => experience.ProfileId);
            profile.HasMany(candidate => candidate.Educations).WithOne().HasForeignKey(education => education.ProfileId);
            profile.HasMany(candidate => candidate.Languages).WithOne().HasForeignKey(language => language.ProfileId);
        });

        modelBuilder.Entity<CvSkill>(skill =>
        {
            skill.Property(candidate => candidate.Category).HasMaxLength(100);
            skill.Property(candidate => candidate.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<CvExperience>(experience =>
        {
            experience.Property(candidate => candidate.Company).HasMaxLength(150);
            experience.Property(candidate => candidate.CompanySummary).HasMaxLength(300);
            experience.Property(candidate => candidate.Role).HasMaxLength(150);
        });

        modelBuilder.Entity<CvEducation>(education =>
        {
            education.Property(candidate => candidate.Institution).HasMaxLength(200);
            education.Property(candidate => candidate.Degree).HasMaxLength(100);
            education.Property(candidate => candidate.Field).HasMaxLength(100);
            education.Property(candidate => candidate.Thesis).HasMaxLength(300);
        });

        modelBuilder.Entity<CvLanguage>(language =>
        {
            language.Property(candidate => candidate.Name).HasMaxLength(50);
            language.Property(candidate => candidate.Level).HasMaxLength(100);
        });

        modelBuilder.Entity<Quote>().Property(candidate => candidate.Text).HasMaxLength(300);
    }
}
