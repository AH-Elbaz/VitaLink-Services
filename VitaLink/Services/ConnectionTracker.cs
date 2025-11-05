using System.Collections.Concurrent;

namespace Vitalink.API.Services
{
    // خدمة Singleton لتخزين الاتصالات النشطة
    public class ConnectionTracker
    {
        // استخدام ConcurrentDictionary لضمان الأمان عند تعدد الخيوط (Thread-Safety)
        // Key: Username (FirstName)
        // Value: ConnectionId (معرف اتصال SignalR المؤقت)
        private static readonly ConcurrentDictionary<string, string> ActiveConnections =
            new ConcurrentDictionary<string, string>();

        /// <summary>
        /// يُضيف أو يُحدّث معرف الاتصال للمستخدم عند تسجيل دخوله للواجهة الأمامية.
        /// </summary>
        public void AddConnection(string username, string connectionId)
        {
            // AddOrUpdate تضيف قيمة جديدة أو تحدث قيمة موجودة
            ActiveConnections.AddOrUpdate(username, connectionId, (key, oldValue) => connectionId);
        }

        /// <summary>
        /// يُزيل معرف الاتصال عند انقطاع اتصال الواجهة الأمامية (تسجيل الخروج أو إغلاق الصفحة).
        /// </summary>
        public void RemoveConnection(string connectionId)
        {
            // نبحث عن اسم المستخدم المرتبط بـ ConnectionId ثم نزيله
            var item = ActiveConnections.FirstOrDefault(x => x.Value == connectionId);
            if (item.Key != null)
            {
                ActiveConnections.TryRemove(item.Key, out _);
            }
        }

        /// <summary>
        /// الحصول على معرف الاتصال النشط بناءً على اسم المستخدم (Username).
        /// </summary>
        public string? GetConnectionId(string username)
        {
            if (ActiveConnections.TryGetValue(username, out string? connectionId))
            {
                return connectionId;
            }
            return null;
        }
    }
}