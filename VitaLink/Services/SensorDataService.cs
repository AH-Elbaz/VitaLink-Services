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
        private readonly IDbContextFactory<VitalinkDbContext> _contextFactory;


        public SensorDataService(IDbContextFactory<VitalinkDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task SaveRowData(SensorDataDto data)
        {
            await using (var dbContext = _contextFactory.CreateDbContext()) {

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

                    dbContext.SensorDataRaw.Add(entity);
                    await dbContext.SaveChangesAsync();

                }
                catch (DbUpdateException ex)
                {

                    Debug.WriteLine($"[DB ERROR] Failed to save sensor data: {ex.Message}");

                    throw;
                }

            }
         
        }







    }
}