
using Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vitalink.API.Dtos;
using Vitalink.API.Services;
using Vitalink.Models;
using VitaLink.Models;
using VitaLink.Models.Data;

namespace Vitalink.API.Controllers
{


    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public class ResetPasswordDto
        {
            public string Username { get; set; }
            public string NewPassword { get; set; }
        
        }
        private readonly VitalinkDbContext _context;
        private readonly ITokenService _tokenService;
     

        public AuthController(VitalinkDbContext context, ITokenService tokenService )
        {
            _context = context;
            _tokenService = tokenService;
            
        }



        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var athlete = await _context.AthleteProfiles.FirstOrDefaultAsync(a => a.FirstName == dto.Username);
            if (athlete == null) return BadRequest("User not found.");

            athlete.PasswordHash = _tokenService.HashPassword(dto.NewPassword);

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Password changed successfully. You can log in now." });
        }


        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto credentials)
        {
            var athlete = await _context.AthleteProfiles
                .FirstOrDefaultAsync(a => a.FirstName == credentials.Username);

       
            if (athlete == null || !_tokenService.VerifyPassword(credentials.Password, athlete.PasswordHash))
            {
                return Unauthorized(new { Message = "Invalid username or password." });
            }

         
            var accessToken = _tokenService.GenerateAccessToken(athlete);
            var refreshToken = _tokenService.GenerateRefreshToken(athlete);
            await _tokenService.SaveRefreshTokenAsync(athlete, refreshToken);

      
            var responseDto = _tokenService.CreateTokenResponseDto(athlete, accessToken, refreshToken);
            return Ok(responseDto);
        }


        [HttpPost("register")]
        [ProducesResponseType(typeof(AthleteProfile), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<AthleteProfile>> Register([FromBody] RegisterDto registerDto)
        {
        
            var existingUser = await _context.AthleteProfiles
                .AnyAsync(a => a.FirstName == registerDto.FirstName);

            if (existingUser)
            {
                return BadRequest(new { Message = "Username (FirstName) is already taken." });
            }

            
            string passwordHash = _tokenService.HashPassword(registerDto.Password);

            var newAthlete = new AthleteProfile
            {
                AthleteID = Guid.NewGuid().ToString(),
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PasswordHash = passwordHash,
                Role = 0,
                BirthDate = registerDto.BirthDate,
                Weight = registerDto.Weight,
                BodyFatPercentage = registerDto.BodyFatPercentage,
                BloodType = registerDto.BloodType,
                TargetSport = registerDto.TargetSport,
               
            };

            _context.AthleteProfiles.Add(newAthlete);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Login), new { username = newAthlete.FirstName }, newAthlete);
        }

     
   

       
       

    }


}
