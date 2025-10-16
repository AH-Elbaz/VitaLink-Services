// ملف: Data/VitalinkDbContext.cs
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
            // تأكيد العلاقة بين الرياضي والتوكن
            modelBuilder.Entity<AthleteProfile>()
                .HasMany(a => a.RefreshTokens)
                .WithOne(t => t.Athlete)
                .HasForeignKey(t => t.AthleteID);

            base.OnModelCreating(modelBuilder);
        }
    }
}