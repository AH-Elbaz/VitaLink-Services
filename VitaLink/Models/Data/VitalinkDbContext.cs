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
        public DbSet<UserBelt> UserBelts { get; set; }


        public DbSet<SensorDataRaw> SensorDataRaw { get; set; }
        public DbSet<AIRecommendation> AIRecommendations { get; set; }

        public DbSet<SessionSummary> SessionSummaries { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserBelt>()
         .HasOne(b => b.Athlete)
         .WithMany(a => a.UserBelts)
         .HasForeignKey(b => b.AthleteID)
         .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserBelt>()
         .HasIndex(b => b.BeltID)
         .IsUnique();


            base.OnModelCreating(modelBuilder);
        }
    }
}