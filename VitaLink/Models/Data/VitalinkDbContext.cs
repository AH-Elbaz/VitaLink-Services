using Microsoft.EntityFrameworkCore;
using Vitalink.Models;

namespace VitaLink.Models.Data
{
    public class VitalinkDbContext : DbContext
    {
        public VitalinkDbContext(DbContextOptions<VitalinkDbContext> options)
            : base(options)
        {
        }

        public DbSet<AthleteProfile> AthleteProfiles { get; set; }
        public DbSet<TrainingSession> TrainingSessions { get; set; }
        public DbSet<SensorDataRaw> SensorDataRaw { get; set; }
        public DbSet<AIRecommendation> AIRecommendations { get; set; }
        public DbSet<SessionSummary> SessionSummaries { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Unique index for BeltID in AthleteProfile
            modelBuilder.Entity<AthleteProfile>()
                .HasIndex(a => a.BeltID)
                .IsUnique();

            // Relationship between AthleteProfile and RefreshToken
            modelBuilder.Entity<AthleteProfile>()
                .HasMany(a => a.RefreshTokens)
                .WithOne(t => t.Athlete)
                .HasForeignKey(t => t.AthleteID);

            modelBuilder.Entity<SensorDataRaw>(entity =>
            {
                entity.HasKey(e => e.Id); // use new Id as PK
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.BeltID).IsRequired();
                entity.Property(e => e.HeartRate).IsRequired();
                entity.Property(e => e.Spo2);
                entity.Property(e => e.Temperature);
                entity.Property(e => e.AccX);
                entity.Property(e => e.AccY);
                entity.Property(e => e.AccZ);
                entity.Property(e => e.Sweat);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
