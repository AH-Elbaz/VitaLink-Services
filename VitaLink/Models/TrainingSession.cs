
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
        public DateTime? EndTime { get; set; }
        public string SportType { get; set; }
        public TimeSpan TotalDuration { get; set; }

        public string AthleteID { get; set; }

        [ForeignKey("AthleteID")]
        public AthleteProfile Athlete { get; set; }

        public ICollection<SensorDataRaw> SensorDataRaw { get; set; } = new List<SensorDataRaw>();
        public ICollection<AIRecommendation> AIRecommendations { get; set; } = new List<AIRecommendation>();

        public SessionSummary? SessionSummary { get; set; }
    }
}