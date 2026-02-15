
using System.ComponentModel.DataAnnotations;
using VitaLink.Models;

namespace Vitalink.Models
{
    public class AthleteProfile
    {
        [Key]
        public string AthleteID { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string PasswordHash { get; set; } = null!;
        public string LastName { get; set; }
        public int Role { get; set; } = 0;


       
        public ICollection<UserBelt> UserBelts { get; set; } = new List<UserBelt>();
 
    }
}