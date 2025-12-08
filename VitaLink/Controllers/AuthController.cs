
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
        private readonly IFaceService _faceService;

        public AuthController(VitalinkDbContext context, ITokenService tokenService , IFaceService faceService )
        {
            _context = context;
            _tokenService = tokenService;
            _faceService = faceService;
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

            Guid? azurePersonId = null;

           
            if (!string.IsNullOrEmpty(registerDto.ProfileImageBase64))
            {
                try
                {
                    using var imageStream = ConvertBase64ToStream(registerDto.ProfileImageBase64);

              
                    var validationStatus = await _faceService.ValidateFaceForRegistration(imageStream);

                    switch (validationStatus)
                    {
                        case FaceRegistrationStatus.NoFaceDetected:
                            return BadRequest(new { Message = "No person detected." });

                        case FaceRegistrationStatus.MultipleFacesDetected:
                            return BadRequest(new { Message = "Multiple faces detected in the frame." });

                        case FaceRegistrationStatus.UserAlreadyExists:
                            return Conflict(new { Message = "This person already has an account." });

                        case FaceRegistrationStatus.Error:
                            return StatusCode(500, new { Message = "Image transmission error." });
                    }

               
                    imageStream.Position = 0;

                 
                    azurePersonId = await _faceService.CreatePersonAndAddFaceAsync(registerDto.FirstName, imageStream);

                    if (azurePersonId == null)
                    {
                        return StatusCode(500, new { Message = "Azure error" });
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Message = "Base64 conversion error." });
                }
            }
            else
            {
                return BadRequest(new { Message = "Face image is required for registration." });
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
                AzurePersonId = azurePersonId
            };

            _context.AthleteProfiles.Add(newAthlete);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Login), new { username = newAthlete.FirstName }, newAthlete);
        }

     
        private Stream ConvertBase64ToStream(string base64)
        {
         
            var base64String = base64;
            if (base64.Contains(","))
            {
                base64String = base64.Split(',')[1];
            }

            var bytes = Convert.FromBase64String(base64String);
            return new MemoryStream(bytes);
        }

        [HttpPost("verify-identity")]
        public async Task<IActionResult> VerifyIdentity([FromBody] VerifyIdentityDto dto)
        {
           
            if (string.IsNullOrEmpty(dto.ImageBase64))
            {
                return BadRequest(new { Message = "Face image is required for verification." });
            }

         
            var athlete = await _context.AthleteProfiles
                .FirstOrDefaultAsync(a => a.FirstName == dto.Username);

          
            if (athlete == null || athlete.AzurePersonId == null)
            {
                return BadRequest(new { Message = "User not found or has no registered face." });
            }

            try
            {
              
                using var imageStream = ConvertBase64ToStream(dto.ImageBase64);

                
                bool isMatch = await _faceService.VerifyUserAsync((Guid)athlete.AzurePersonId, imageStream);

                if (isMatch)
                {
                    return Ok(new
                    {
                        Message = "Identity verified.",
                        IsVerified = true,
                    });
                }
                else
                {
                    return Unauthorized(new { Message = "Verification failed. The face does not match the account owner." });
                }
            }
            catch (Exception)
            {
                return BadRequest(new { Message = "An error occurred while processing the image. Make sure a valid image is provided." });
            }
        }

    }


}
