using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Collections.Concurrent; // أضف هذا المسار
using Vitalink.API.Dtos;
using Vitalink.API.Services;
using VitaLink.Models.Data;

namespace Vitalink.API.Hubs
{
    public class SensorDataHub : Hub
    {
        private readonly ConnectionTracker _tracker;
        private readonly ISensorDataService _sensorDataService;
        private readonly IDbContextFactory<VitalinkDbContext> _contextFactory;

        // قاموس لتخزين آخر قيم فعلية (غير صفرية) لكل حزام
        private static readonly ConcurrentDictionary<string, SensorDataDto> LastValidData = new ConcurrentDictionary<string, SensorDataDto>();

        public SensorDataHub(ConnectionTracker tracker, ISensorDataService sensorDataService, IDbContextFactory<VitalinkDbContext> contextFactory)
        {
            _tracker = tracker;
            _sensorDataService = sensorDataService;
            _contextFactory = contextFactory;
        }

        public async Task RegisterConnection(string username)
        {
            _tracker.AddConnection(username, Context.ConnectionId);
            Debug.WriteLine($"[CONNECTION] User {username} registered ID: {Context.ConnectionId}");
        }

        public async Task SendSensorData(SensorDataDto data)
        {
            // معالجة مشكلة الأصفار قبل الحفظ أو البث
            ProcessSensorZeros(data);

            var incomingBeltId = data.BeltID;
            string? targetUsername;

            await using (var dbContext = _contextFactory.CreateDbContext())
            {
                targetUsername = await dbContext.AthleteProfiles
                                                .Where(a => a.BeltID == incomingBeltId)
                                                .Select(a => a.FirstName)
                                                .FirstOrDefaultAsync();
            }

            if (targetUsername != null)
            {
                await _sensorDataService.SaveRowData(data);
                var targetConnectionIds = _tracker.GetConnectionIds(targetUsername);

                if (targetConnectionIds.Any())
                {
                    await Clients.Clients(targetConnectionIds.ToList()).SendAsync("ReceiveLiveUpdate", data);
                }
            }
        }

   
        private void ProcessSensorZeros(SensorDataDto currentData)
        {
            var beltId = currentData.BeltID;

            
            if (!LastValidData.TryGetValue(beltId, out var lastValid))
            {
                if (currentData.HeartRate > 0 || currentData.Spo2 > 0 || currentData.Temperature > 0)
                {
                    LastValidData[beltId] = new SensorDataDto
                    {
                        HeartRate = currentData.HeartRate,
                        Spo2 = currentData.Spo2,
                        Temperature = currentData.Temperature
                    };
                }
                return;
            }

           
            if (currentData.HeartRate <= 0 && lastValid.HeartRate > 0)
                currentData.HeartRate = lastValid.HeartRate;
            else if (currentData.HeartRate > 0)
                lastValid.HeartRate = currentData.HeartRate;

            
            if (currentData.Spo2 <= 0 && lastValid.Spo2 > 0)
                currentData.Spo2 = lastValid.Spo2;
            else if (currentData.Spo2 > 0)
                lastValid.Spo2 = currentData.Spo2;

           
            if (currentData.Temperature <= 0 && lastValid.Temperature > 0)
                currentData.Temperature = lastValid.Temperature;
            else if (currentData.Temperature > 0)
                lastValid.Temperature = currentData.Temperature;

            LastValidData[beltId] = lastValid;
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _tracker.RemoveConnection(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}