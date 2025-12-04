// ملف: Models/AIRecommendation.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vitalink.Models
{
    public class AIRecommendation
    {
        [Key]
        public int RecommendationID { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public double FatigueLevel { get; set; }
        public double OptimalIntensity { get; set; }
        public string AlertMessage { get; set; }


        // المفتاح الخارجي لربط التوصية بالجلسة التي حدثت فيها
        public int SessionID { get; set; }

        [ForeignKey("SessionID")]
        public TrainingSession TrainingSession { get; set; }
    }
}