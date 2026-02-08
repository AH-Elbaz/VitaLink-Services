
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vitalink.Models;

namespace Vitalink.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Token { get; set; } = null!;

        public DateTime ExpiryDate { get; set; }

        [Required]
        public string AthleteID { get; set; } = null!; 

        [ForeignKey("AthleteID")]
        public AthleteProfile Athlete { get; set; } = null!;
    }
}