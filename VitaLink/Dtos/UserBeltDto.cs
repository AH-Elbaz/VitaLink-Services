using System.ComponentModel.DataAnnotations;

namespace Dtos
{

    public class CreateUserBeltDto
    {
        public string BeltID { get; set; }
        public string AthleteID { get; set; }
        public string name { get; set; }
        public IFormFile? ProfileImage { get; set; } 
        public DateTime BirthDate { get; set; }
        public double Weight { get; set; }
        public string BloodType { get; set; }
        public double BodyFatPercentage { get; set; }
        public string TargetSport { get; set; }
    }


    public class UserBeltResponseDto
    {
        public string BeltID { get; set; }
        public string AthleteID { get; set; }
        public string name { get; set; }
        public string? ProfileImage { get; set; } 
        public DateTime BirthDate { get; set; }
        public double Weight { get; set; }
        public string BloodType { get; set; }
        public double BodyFatPercentage { get; set; }
        public string TargetSport { get; set; }
    }
}
