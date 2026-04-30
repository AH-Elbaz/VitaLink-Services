using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaLink.Models.Data;
using Vitalink.Models;
using Vitalink.API.Dtos;
using VitaLink.Models;
using Dtos;



namespace Vitalink.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AthleteProfilesController : ControllerBase
    {
      
        private readonly VitalinkDbContext _context;

        
        public AthleteProfilesController(VitalinkDbContext context)
        {
            _context = context;
        }


        [HttpGet("Getuserbelt")]
        public async Task<ActionResult> getUserBelt(string name)
        {
            var userBelts = await _context.UserBelts
                .Include(ub => ub.Athlete)
                .Where(ub => ub.Athlete.FirstName == name)
                .Select(ub => new UserBeltResponseDto
                {
                    BeltID = ub.BeltID,
                    AthleteID = ub.AthleteID,
                    name = ub.name,
                
                    ProfileImage = ub.ProfileImage != null ? Convert.ToBase64String(ub.ProfileImage) : null,
                    BirthDate = ub.BirthDate,
                    Weight = ub.Weight,
                    BloodType = ub.BloodType,
                    BodyFatPercentage = ub.BodyFatPercentage,
                    TargetSport = ub.TargetSport
                })
                .ToListAsync();

            if (userBelts == null || !userBelts.Any())
            {
                return NotFound("No belts found for the given athlete name.");
            }

            return Ok(userBelts);
        }


        [HttpPost("userbilt")]

        public async Task<ActionResult> postBuilt([FromForm] CreateUserBeltDto userBeltDto)
        {
            byte[]? imageBytes = null;

            // ????? ??? IFormFile ??? byte[] ??????? ?? ????? ????????
            if (userBeltDto.ProfileImage != null && userBeltDto.ProfileImage.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await userBeltDto.ProfileImage.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }

            UserBelt newUserBelt = new UserBelt
            {
                BeltID = userBeltDto.BeltID,
                AthleteID = userBeltDto.AthleteID,
                name = userBeltDto.name,
                ProfileImage = imageBytes, 
                BirthDate = userBeltDto.BirthDate,
                Weight = userBeltDto.Weight,
                BloodType = userBeltDto.BloodType,
                BodyFatPercentage = userBeltDto.BodyFatPercentage,
                TargetSport = userBeltDto.TargetSport
            };

            _context.UserBelts.Add(newUserBelt);
            await _context.SaveChangesAsync();
            return Ok("Belt created successfully.");
        }


        [HttpPost]
        public async Task<ActionResult<AthleteProfile>> PostAthleteProfile(AthleteProfile athleteProfile)
        {
            if (string.IsNullOrEmpty(athleteProfile.AthleteID))
            {
                athleteProfile.AthleteID = Guid.NewGuid().ToString();
            }

         
            _context.AthleteProfiles.Add(athleteProfile);

        
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAthleteProfile), new { id = athleteProfile.AthleteID }, athleteProfile);
        }

        [HttpGet("sensorData")]
        public async Task<ActionResult<IEnumerable<AthleteProfile>>> getRowData()
        {
            
            if (_context.AthleteProfiles == null)
            {
                return NotFound();
            }

            var data = await _context.SensorDataRaw.Select(l => new SensorDataDto
            {

                BeltID = l.BeltID,
                HeartRate = l.HeartRate,
                Spo2 = l.Spo2,
                Temperature = l.Temperature,
                AccX = l.AccX,
                AccY = l.AccY,
                AccZ = l.AccZ,
                Sweat = l.Sweat
            }).ToListAsync();
            return Ok(data);
        }

        [HttpDelete("delete")]

      public async Task<IActionResult> DeleteSensorsData()
        {
            if (_context.SensorDataRaw == null)
            {
                return NotFound();
            }
            
            _context.SensorDataRaw.RemoveRange(_context.SensorDataRaw);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<AthleteProfile>>> GetAthleteProfiles()
        {
            
            if (_context.AthleteProfiles == null)
            {
                return NotFound();
            }
            var athe = await _context.AthleteProfiles.Include(a => a.UserBelts).ToListAsync();

            return athe;
        }

  
        [HttpGet("{id}")]
        public async Task<ActionResult<AthleteProfile>> GetAthleteProfile(string id)
        {
            var athleteProfile = await _context.AthleteProfiles.FindAsync(id);

            if (athleteProfile == null)
            {
                return NotFound();
            }

            return athleteProfile;
        }


        [HttpGet("GetAllBelt")]
        public async Task<ActionResult> getalldevices()
        {
            var devices = await _context.UserBelts.ToListAsync();
            return Ok(devices);
        }
    }
}