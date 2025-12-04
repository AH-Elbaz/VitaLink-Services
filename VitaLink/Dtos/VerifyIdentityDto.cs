// Dtos/VerifyIdentityDto.cs
using System.ComponentModel.DataAnnotations;

namespace Dtos
{
    public class VerifyIdentityDto
    {
        [Required]
        public string Username { get; set; } // أو FirstName حسب نظامك

        [Required]
        public string ImageBase64 { get; set; } // صورة السيلفي الجديدة
    }
}