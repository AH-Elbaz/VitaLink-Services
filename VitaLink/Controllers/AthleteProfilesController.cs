using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vitalink.API.Dtos;
using Vitalink.Models;
using VitaLink.Models.Data;


namespace Vitalink.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AthleteProfilesController : ControllerBase
    {
        // Field to hold the database context
        private readonly VitalinkDbContext _context;

        // Constructor: Dependency Injection of DbContext
        public AthleteProfilesController(VitalinkDbContext context)
        {
            _context = context;
        }

        // -------------------------------------------------------------------
        // HTTP POST: api/AthleteProfiles
        // Adds a new athlete profile to the database.
        // -------------------------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<AthleteProfile>> PostAthleteProfile(AthleteProfile athleteProfile)
        {
            // Set the ID if it's not set (using the Guid generation logic from the Model)
            if (string.IsNullOrEmpty(athleteProfile.AthleteID))
            {
                athleteProfile.AthleteID = Guid.NewGuid().ToString();
            }

            // 1. Add the new object to the context
            _context.AthleteProfiles.Add(athleteProfile);

            // 2. Save the changes to the SQL Server database
            await _context.SaveChangesAsync();

            // 3. Return a success status with the created object and the location of the new resource
            return CreatedAtAction(nameof(GetAthleteProfile), new { id = athleteProfile.AthleteID }, athleteProfile);
        }

        [HttpPost("raw")]
        public async Task<IActionResult> RawData([FromBody] SensorDataDto data)
        {
            if (data == null)
                return BadRequest("Sensor data cannot be null.");

            var entity = new SensorDataRaw
            {
                BeltID = data.BeltID,
                HeartRate = data.HeartRate,
                Spo2 = data.Spo2,
                Temperature = data.Temperature,
                AccX = data.AccX,
                AccY = data.AccY,
                AccZ = data.AccZ,
                Sweat = data.Sweat,
                Timestamp = DateTime.UtcNow
            };

            _context.SensorDataRaw.Add(entity);
            await _context.SaveChangesAsync();

            return Ok();
        }


        // -------------------------------------------------------------------
        // HTTP GET: api/AthleteProfiles
        // Retrieves a list of all athlete profiles.
        // -------------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AthleteProfile>>> GetAthleteProfiles()
        {
            // Check if the table is accessible
            if (_context.AthleteProfiles == null)
            {
                return NotFound();
            }

            // Return all profiles from the database
            return await _context.AthleteProfiles.ToListAsync();
        }

        // Helper GET method (required for CreatedAtAction in POST)
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
    }
}
