using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vitalink.Models
{
    public class SensorDataRaw
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // إضافة معرف تلقائي لكل سطر قراءة

        [Required]
        public string BeltID { get; set; } // سيبقى المعرف القادم من الحزام

        public float HeartRate { get; set; }
        public byte Spo2 { get; set; }
        public float Temperature { get; set; }
        public float AccX { get; set; }
        public float AccY { get; set; }
        public float AccZ { get; set; }
        public ushort Sweat { get; set; }
     
    }
}
