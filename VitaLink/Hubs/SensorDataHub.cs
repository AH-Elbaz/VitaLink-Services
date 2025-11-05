using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Vitalink.API.Dtos;
using Vitalink.API.Services;
using VitaLink.Models.Data;

namespace Vitalink.API.Hubs
{
    // نفترض أن SensorDataDto يحتوي على خاصية BeltID
    public class SensorDataHub : Hub
    {
        private readonly ConnectionTracker _tracker;
        private readonly VitalinkDbContext _dbContext; // للوصول لقاعدة البيانات

        public SensorDataHub(ConnectionTracker tracker, VitalinkDbContext dbContext)
        {
            _tracker = tracker;
            _dbContext = dbContext;
        }

        // ***************************************************************
        // 1. وظيفة تسجيل الاتصال (يتم استدعاؤها من الواجهة الأمامية)
        // ***************************************************************

        // يتم استدعاؤها من Frontend فور تسجيل الدخول بنجاح
        // 'username' في هذه الحالة هو الـ FirstName الذي تستخدمه للمصادقة
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
            var incomingBeltId = data.BeltID; // استخراج BeltID المرسل من ESP32

            if (string.IsNullOrEmpty(incomingBeltId))
            {
                Debug.WriteLine("[ERROR] Received data with null or empty BeltID.");
                return;
            }

            // 1. البحث في قاعدة البيانات عن المستخدم المرتبط بهذا الحزام
            var athlete = await _dbContext.AthleteProfiles
                                          .Where(a => a.BeltID == incomingBeltId)
                                          .Select(a => a.FirstName) // اختيار الـ Username (الـ FirstName)
                                          .FirstOrDefaultAsync();

            if (athlete != null)
            {
                // 2. الحصول على الـ ConnectionID النشط حالياً للمستخدم
                var targetConnectionId = _tracker.GetConnectionId(athlete);

                if (targetConnectionId != null)
                {
                    // 3. توجيه البيانات مباشرة إلى اتصال المستخدم المحدد
                    await Clients.Client(targetConnectionId).SendAsync("ReceiveLiveUpdate", data);
                    Debug.WriteLine($"[STREAM] Data routed from {incomingBeltId} to user {athlete} via {targetConnectionId}");
                }
                else
                {
                    Debug.WriteLine($"[WARNING] Data received for {athlete} but dashboard is not connected.");
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