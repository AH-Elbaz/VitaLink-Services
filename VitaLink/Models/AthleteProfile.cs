// ملف: Models/AthleteProfile.cs
using System.ComponentModel.DataAnnotations;

namespace Vitalink.Models
{
    public class AthleteProfile
    {
        [Key] // يحدد أن هذا هو المفتاح الأساسي
        public string AthleteID { get; set; } = Guid.NewGuid().ToString(); // استخدام GUID كسلسلة لسهولة الإنشاء

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string PasswordHash { get; set; } = null!;
        public string LastName { get; set; }
        public int Role { get; set; } = 0;

        public DateTime BirthDate { get; set; }
        public double Weight { get; set; } // كجم
        public string BloodType { get; set; }
        public double BodyFatPercentage { get; set; }
        public string TargetSport { get; set; }

        // خاصية للملاحة (Navigation Property) لربط الرياضي بجلساته
        public ICollection<TrainingSession> TrainingSessions { get; set; } = new List<TrainingSession>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}