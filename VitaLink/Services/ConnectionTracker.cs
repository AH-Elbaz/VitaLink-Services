// ملف: Services/ConnectionTracker.cs

using System.Collections.Concurrent;

namespace Vitalink.API.Services
{
    // يجب تسجيل هذه الخدمة كـ Singleton في Program.cs
    public class ConnectionTracker
    {
        // ConcurrentDictionary لضمان التخزين الآمن للخيوط
        // Key: Username (FirstName), Value: ConnectionId (معرف اتصال SignalR المؤقت)
        private static readonly ConcurrentDictionary<string, string> ActiveConnections =
            new ConcurrentDictionary<string, string>();

        /// <summary>
        /// يُضيف أو يُحدّث معرف الاتصال للمستخدم.
        /// </summary>
        public void AddConnection(string username, string connectionId)
        {
            ActiveConnections.AddOrUpdate(username, connectionId, (key, oldValue) => connectionId);
        }

        /// <summary>
        /// يُزيل معرف الاتصال عند انقطاعه.
        /// </summary>
        public void RemoveConnection(string connectionId)
        {
            // يبحث عن القيمة (ConnectionId) ليجد الـ Key (Username) ثم يزيله
            var item = ActiveConnections.FirstOrDefault(x => x.Value == connectionId);
            if (item.Key != null)
            {
                ActiveConnections.TryRemove(item.Key, out _);
            }
        }

        /// <summary>
        /// الحصول على معرف الاتصال النشط بناءً على اسم المستخدم.
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