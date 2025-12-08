// ملف: Models/AthleteProfile.cs
using System.ComponentModel.DataAnnotations;

namespace Vitalink.Models
{
    public class AthleteProfile
    {
        [Key]
        public string AthleteID { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string PasswordHash { get; set; } = null!;
        public string LastName { get; set; }
        public int Role { get; set; } = 0;
        public DateTime BirthDate { get; set; }
        public double Weight { get; set; }
        public string BloodType { get; set; }
        public double BodyFatPercentage { get; set; }
        public string TargetSport { get; set; }

        public string? BeltID { get; set; }

        public Guid? AzurePersonId { get; set; }

        public ICollection<TrainingSession> TrainingSessions { get; set; } = new List<TrainingSession>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}