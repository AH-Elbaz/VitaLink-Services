using System.ComponentModel.DataAnnotations;

namespace Vitalink.API.Dtos
{
    public class RegisterDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        public double Weight { get; set; }

        public double BodyFatPercentage { get; set; }

        [StringLength(5)]
        public string? BloodType { get; set; }


        [StringLength(50)]
        public string? TargetSport { get; set; }
    }
}