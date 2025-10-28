using System.ComponentModel.DataAnnotations;

namespace Vitalink.API.Dtos
{
    public class SensorDataDto
    {
       
        //[Required]
        //public string AthleteID { get; set; } = null!;

        //[Required]
        //public DateTime Timestamp { get; set; } = DateTime.UtcNow;


  
        [Required]
        public float HeartRate { get; set; } 
     

        public byte Spo2 { get; set; }

       
        public float Temperature { get; set; }


        public float AccX { get; set; }
        public float AccY { get; set; } 
        public float AccZ { get; set; } 

     
        public ushort Sweat { get; set; }
    }
}