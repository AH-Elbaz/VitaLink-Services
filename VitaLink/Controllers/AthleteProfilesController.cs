using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaLink.Models.Data;
using Vitalink.Models;
using Vitalink.API.Dtos;


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



     
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AthleteProfile>>> GetAthleteProfiles()
        {
            
            if (_context.AthleteProfiles == null)
            {
                return NotFound();
            }

           
            return await _context.AthleteProfiles.ToListAsync();
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
    }
}