using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Vitalink.API.Dtos;
using Vitalink.API.Services;
using VitaLink.Models.Data;
using Microsoft.Extensions.DependencyInjection; // Required for IServiceScopeFactory
using System;

namespace Vitalink.API.Hubs
{
    public class SensorDataHub : Hub
    {
        private readonly ConnectionTracker _tracker;
        private readonly IDbContextFactory<VitalinkDbContext> _contextFactory;
        private readonly IServiceScopeFactory _serviceScopeFactory; // Added IServiceScopeFactory

        public SensorDataHub(ConnectionTracker tracker, IDbContextFactory<VitalinkDbContext> contextFactory, IServiceScopeFactory serviceScopeFactory)
        {
            _tracker = tracker;
            _contextFactory = contextFactory;
            _serviceScopeFactory = serviceScopeFactory; // Initialize IServiceScopeFactory
        }

        public async Task RegisterConnection(string username)
        {
            _tracker.AddConnection(username, Context.ConnectionId);
            Debug.WriteLine($"[CONNECTION] User {username} registered ID: {Context.ConnectionId}");
        }

        public async Task SendSensorData(SensorDataDto data)
        {
            var incomingBeltId = data.BeltID;
            string? targetUsername;

            // 1. البحث عن المستخدم المرتبط بالحزام
            await using (var dbContext = _contextFactory.CreateDbContext())
            {
                targetUsername = await dbContext.AthleteProfiles
                                                .Where(a => a.BeltID == incomingBeltId)
                                                .Select(a => a.FirstName)
                                                .FirstOrDefaultAsync();
            }

            if (targetUsername != null)
            {
                // 2. إرسال التحديث المباشر فوراً للمستخدمين المتصلين
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

                // 3. تشغيل عملية الحفظ في الخلفية باستخدام نطاق خدمة خاص بها
                // هذا يضمن أن الخدمات المحقونة (مثل DbContext و ISensorDataService) لها دورة حياة خاصة بها
                // ولا تتأثر بدورة حياة الـ Hub المؤقتة.
                _ = Task.Run(async () =>
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var sensorDataService = scope.ServiceProvider.GetRequiredService<ISensorDataService>();
                        try
                        {
                            await sensorDataService.SaveRowData(data);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[ERROR] Failed to save sensor data in background: {ex.Message}");
                            // يمكنك هنا إضافة المزيد من معالجة الأخطاء، مثل تسجيلها في نظام تسجيل الأخطاء
                        }
                    }
                });
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
