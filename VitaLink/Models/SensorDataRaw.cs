// ملف: Models/SensorDataRaw.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vitalink.Models
{
    public class SensorDataRaw
    {
        [Key]
        public long DataID { get; set; } // نستخدم long لأن هذا الجدول سيكون كبيراً جداً

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // بيانات الحساسات
        public int HeartRate { get; set; }
        public double OxygenSaturation { get; set; }
        public double BodyTemperature { get; set; }
        public string SweatComposition { get; set; } // مثال على بيانات متغيرة
        public double MotionData_X { get; set; }

        // المفتاح الخارجي لربط البيانات بجلسة التدريب
        public int SessionID { get; set; }

        [ForeignKey("SessionID")]
        public TrainingSession TrainingSession { get; set; }
    }
}