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

        [HttpPost("userbilt")]
        public async Task<ActionResult> postBuilt(UserBeltDto userBelt)
        {
            UserBelt newUserBelt = new UserBelt
            {
                BeltID = userBelt.BeltID,
                AthleteID = userBelt.AthleteID
            };
            _context.UserBelts.Add(newUserBelt);
            await _context.SaveChangesAsync();
            return Ok();
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
            var devices = await _context.UserBelts.Select(a => a.BeltID).ToListAsync();
            return Ok(devices);
        }
    }
}