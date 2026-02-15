using System.ComponentModel.DataAnnotations;

namespace Dtos
{
    public class UserBeltDto
    {
        [Key]
        public string BeltID { get; set; }
        [Required]
        public string AthleteID { get; set; }

        [Required]
        public string name { get; set; }
        public byte[]? ProfileImage { get; set; }
        public DateTime BirthDate { get; set; }
        public double Weight { get; set; }
        public string BloodType { get; set; }
        public double BodyFatPercentage { get; set; }
        public string TargetSport { get; set; }
    }
}
