<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;
=======
using System.ComponentModel.DataAnnotations;
using Vitalink.Models;
>>>>>>> 3d587eb1573481c2dbbedfcd85b2caea70ae95cf

namespace Vitalink.API.Dtos
{
    public class SensorDataDto
    {

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
    }
}
