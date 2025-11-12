using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;
using Vitalink.API.Dtos;
using Vitalink.Models;
using VitaLink.Models.Data;

namespace Vitalink.API.Services
{
    public class SensorDataService : ISensorDataService
    {
        private readonly VitalinkDbContext _dbContext;

    
        public SensorDataService(VitalinkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveRowData(SensorDataDto data)
        {

            var entity = new SensorDataRaw
            {
                AccX = data.AccX,
                AccY = data.AccY,
                AccZ = data.AccZ,
                BeltID = data.BeltID,
                Sweat = data.Sweat,
                HeartRate = data.HeartRate,
                Spo2 = data.Spo2,
                Temperature = data.Temperature
            };

            try
            {
                
                _dbContext.SensorDataRaw.Add(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                
                Debug.WriteLine($"[DB ERROR] Failed to save sensor data: {ex.Message}");
              
                throw;
            }
        }
    }
}