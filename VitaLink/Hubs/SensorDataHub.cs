using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Vitalink.API.Dtos;
using Vitalink.API.Services;
using VitaLink.Models.Data; // تأكد من أن هذا هو المسار الصحيح لـ AthleteProfiles

namespace Vitalink.API.Hubs
{
    public class SensorDataHub : Hub
    {
        private readonly ConnectionTracker _tracker;
        // تم إزالة VitalinkDbContext لأنه لم يعد ضروريًا هنا
        private readonly ISensorDataService _sensorDataService;

        // تم تعديل الـ Constructor لإزالة VitalinkDbContext
        public SensorDataHub(ConnectionTracker tracker, ISensorDataService sensorDataService)
        {
            _tracker = tracker;
            _sensorDataService = sensorDataService;
        }

        // ملاحظة: بما أنك تستخدم _dbContext في دالة SendSensorData للبحث عن targetUsername،
        // يجب أن نستخدم IDbContextFactory هنا أيضًا، أو نمرر الخدمة التي تقوم بالبحث.
        // الحل الأفضل هو استخدام IDbContextFactory في الـ Hub أيضًا للبحث.

        // بما أنك لم ترسل كود البحث، سأفترض أنك قمت بتعديل الـ Hub ليعتمد على IDbContextFactory
        // أو أنك قمت بحقن VitalinkDbContext في الـ Hub (وهو ما قد يسبب المشكلة).

        // سأفترض أنك تريد استخدام IDbContextFactory في الـ Hub للبحث عن اسم المستخدم أيضًا،
        // لتجنب أي مشاكل في التزامن.

        private readonly IDbContextFactory<VitalinkDbContext> _contextFactory;

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
            await using (var dbContext = _contextFactory.CreateDbContext())
            {
                targetUsername = await dbContext.AthleteProfiles
                                                .Where(a => a.BeltID == incomingBeltId)
                                                .Select(a => a.FirstName)
                                                .FirstOrDefaultAsync();
            }


            if (targetUsername != null)
            {
                // استدعاء الخدمة لحفظ البيانات (وهي تستخدم IDbContextFactory أيضًا)
                await _sensorDataService.SaveRowData(data);

                // استمرار بث البيانات
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
