    // ملف: Hubs/SensorDataHub.cs

    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using System.Diagnostics;
    using System.Security.Claims; // للحصول على الهوية
    using Vitalink.API.Dtos;
    using Vitalink.API.Services;
    using VitaLink.Models.Data;

    namespace Vitalink.API.Hubs
    {
        public class SensorDataHub : Hub
        {
            private readonly ConnectionTracker _tracker;
            private readonly VitalinkDbContext _dbContext; // للوصول لقاعدة البيانات وربط BeltID بالـ Username

            // حقن الخدمات المطلوبة
            public SensorDataHub(ConnectionTracker tracker, VitalinkDbContext dbContext)
            {
                _tracker = tracker;
                _dbContext = dbContext;
            }

            // ***************************************************************
            // 1. وظيفة تسجيل الاتصال (يتم استدعاؤها من الواجهة الأمامية)
            // ***************************************************************

            public async Task RegisterConnection(string username)
            {
                // حفظ الربط بين اسم المستخدم ومعرف الاتصال الحالي في التراكر
                _tracker.AddConnection(username, Context.ConnectionId);
                Debug.WriteLine($"[CONNECTION] User {username} registered ID: {Context.ConnectionId}");
            }

            // ***************************************************************
            // 2. وظيفة استقبال وتوجيه بيانات الحزام (يتم استدعاؤها من ESP32)
            // ***************************************************************

            // يجب أن تتأكد من أن حمولة ESP32 (SensorDataDto) تحتوي على خاصية BeltID
            public async Task SendSensorData(SensorDataDto data)
            {
                // 1. التحقق من اكتمال البيانات
              
                var incomingBeltId = data.BeltID;

                // 2. البحث في قاعدة البيانات عن المستخدم المرتبط بهذا الحزام (باستخدام BeltID)
                var targetUsername = await _dbContext.AthleteProfiles
                                              .Where(a => a.BeltID == incomingBeltId)
                                              .Select(a => a.FirstName) // FirstName هو Username المطلوب
                                              .FirstOrDefaultAsync();

                if (targetUsername != null)
                {
                    // 3. الحصول على الـ ConnectionID النشط حالياً للمستخدم
                    var targetConnectionId = _tracker.GetConnectionId(targetUsername);

                    if (targetConnectionId != null)
                    {
                        // 4. توجيه البيانات مباشرة إلى اتصال المستخدم المحدد
                        await Clients.Client(targetConnectionId).SendAsync("ReceiveLiveUpdate", data);
                        Debug.WriteLine($"[STREAM] Data routed from {incomingBeltId} to user {targetUsername} via {targetConnectionId}");
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

            // ***************************************************************
            // 3. تتبع الانقطاع (لإزالة الاتصال)
            // ***************************************************************

            public override async Task OnDisconnectedAsync(Exception? exception)
            {
                // إزالة الـ ConnectionId من التراكر عند انقطاع الاتصال (مهم جداً!)
                _tracker.RemoveConnection(Context.ConnectionId);
                Debug.WriteLine($"[DISCONNECT] Connection ID {Context.ConnectionId} removed.");
                await base.OnDisconnectedAsync(exception);
            }
        }
    }