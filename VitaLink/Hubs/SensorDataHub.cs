using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Vitalink.API.Dtos;
using Vitalink.API.Services;
using VitaLink.Models.Data;

namespace Vitalink.API.Hubs
{
    public class SensorDataHub : Hub
    {
        private readonly ConnectionTracker _tracker;
        private readonly ISensorDataService _sensorDataService;
        private readonly IDbContextFactory<VitalinkDbContext> _contextFactory; // تم الإبقاء على هذا

        // Constructor المصحح: يعتمد على الخدمات التي تم تسجيلها في Program.cs
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
            var incomingBeltId = data.BeltID;

            // استخدام IDbContextFactory للبحث عن اسم المستخدم
            string? targetUsername;
            // استخدام await using لضمان التخلص من السياق بعد الانتهاء
            await using (var dbContext = _contextFactory.CreateDbContext())
            {
                // البحث عن اسم المستخدم
                targetUsername = await dbContext.AthleteProfiles
                                                .Where(a => a.BeltID == incomingBeltId)
                                                .Select(a => a.FirstName)
                                                .FirstOrDefaultAsync();
            }


            if (targetUsername != null)
            {
                // حفظ البيانات باستخدام الخدمة
                await _sensorDataService.SaveRowData(data);
                // await _sensorDataService.SaveRowData(data);

                // بث البيانات
                var targetConnectionIds = _tracker.GetConnectionIds(targetUsername);

                if (targetConnectionIds.Any())
                {
                    await Clients.Clients(targetConnectionIds.ToList()).SendAsync("ReceiveLiveUpdate", data);
                    Debug.WriteLine($"[STREAM SUCCESS] Data routed to {targetConnectionIds.Count()} connection(s) for user {targetUsername}.");
                }
                else
                {
                    Debug.WriteLine($"[WARNING] Data received for {targetUsername} but dashboard is not connected.");
                }
            }
            else
            {
                Debug.WriteLine($"[WARNING] Data received from unknown BeltID: {incomingBeltId}. Ignoring.");
            }
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _tracker.RemoveConnection(Context.ConnectionId);
            Debug.WriteLine($"[DISCONNECT] Connection ID {Context.ConnectionId} removed.");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
