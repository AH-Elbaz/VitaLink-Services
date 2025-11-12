using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vitalink.Models
{
    public class SensorDataRaw
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // unique key for each reading

        [Required]
        public string BeltID { get; set; }

        [Required]
        public float HeartRate { get; set; }

        public byte Spo2 { get; set; }
        public float Temperature { get; set; }
        public float AccX { get; set; }
        public float AccY { get; set; }
        public float AccZ { get; set; }
        public ushort Sweat { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
