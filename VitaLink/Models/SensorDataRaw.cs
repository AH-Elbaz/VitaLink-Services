using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vitalink.Models
{
    public class SensorDataRaw
    {
        [Key]
<<<<<<< HEAD
        public long DataID { get; set; } // نستخدم long لأن هذا الجدول سيكون كبيراً جداً
=======
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // unique key for each reading
>>>>>>> 3d587eb1573481c2dbbedfcd85b2caea70ae95cf

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

<<<<<<< HEAD
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
=======
        [Required]
        public float HeartRate { get; set; }

        public byte Spo2 { get; set; }
        public float Temperature { get; set; }
        public float AccX { get; set; }
        public float AccY { get; set; }
        public float AccZ { get; set; }
        public ushort Sweat { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
>>>>>>> 3d587eb1573481c2dbbedfcd85b2caea70ae95cf
    }
}
