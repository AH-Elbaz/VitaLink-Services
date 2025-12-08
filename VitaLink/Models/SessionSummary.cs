// ملف: Models/SessionSummary.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Vitalink.Models
{
    public class SessionSummary
    {
        [Key]
        public int SummaryID { get; set; }

        [Required]
        public int SessionID { get; set; }

        public double AvgHeartRate { get; set; }
        public double MaxHeartRate { get; set; }
        public double CaloriesBurned { get; set; }
        public double FatigueScore_Final { get; set; }
        public double MaxMotionIntensity { get; set; }

        [Required]
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;

        [ForeignKey("SessionID")]
        public TrainingSession TrainingSession { get; set; }
    }
}