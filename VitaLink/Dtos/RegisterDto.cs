using System.ComponentModel.DataAnnotations;

namespace Vitalink.API.Dtos
{
    public class RegisterDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; } // سيُستخدم كـ Username

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } // كلمة المرور الأصلية (Plain Text)

        [Required]
        public DateTime BirthDate { get; set; }

        public double Weight { get; set; } // الوزن (كجم)

        public double BodyFatPercentage { get; set; } // نسبة الدهون (%)

        [StringLength(5)]
        public string? BloodType { get; set; } // فصيلة الدم

        [StringLength(50)]
        public string? TargetSport { get; set; } // الرياضة المستهدفة
    }
}