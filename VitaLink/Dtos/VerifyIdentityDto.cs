// Dtos/VerifyIdentityDto.cs
using System.ComponentModel.DataAnnotations;

namespace Dtos
{
    public class VerifyIdentityDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string ImageBase64 { get; set; }
    }
}