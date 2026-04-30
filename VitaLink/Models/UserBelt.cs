using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vitalink.Models;

namespace VitaLink.Models
{
    public class UserBelt
    {
        [Key]
        public string BeltID { get; set; }
  

        [Required]
        public string name { get; set; }

        [Required]
        public string AthleteID { get; set; } 

        [ForeignKey("AthleteID")]
        public AthleteProfile Athlete { get; set; }

        public byte[]? ProfileImage { get; set; }
        public DateTime BirthDate { get; set; }
        public double Weight { get; set; }
        public string BloodType { get; set; }
        public double BodyFatPercentage { get; set; }
        public string TargetSport { get; set; }

    }
}
