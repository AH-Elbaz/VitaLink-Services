// ملف: Models/TrainingSession.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vitalink.Models;

namespace Vitalink.Models
{
    public class TrainingSession
    {
        [Key]
        public int SessionID { get; set; }

        [Required]
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? EndTime { get; set; } // يمكن أن تكون فارغة حتى انتهاء الجلسة
        public string SportType { get; set; }
        public TimeSpan TotalDuration { get; set; }

        // المفتاح الخارجي (Foreign Key) لربط الجلسة بالرياضي
        public string AthleteID { get; set; }

        [ForeignKey("AthleteID")]
        public AthleteProfile Athlete { get; set; } // خاصية الملاحة للوصول إلى بيانات الرياضي

        // خصائص الملاحة لربط البيانات الأخرى
        public ICollection<SensorDataRaw> SensorDataRaw { get; set; } = new List<SensorDataRaw>();
        public ICollection<AIRecommendation> AIRecommendations { get; set; } = new List<AIRecommendation>();

        public SessionSummary? SessionSummary { get; set; }
    }
}