// ملف: Services/ConnectionTracker.cs (التعديل النهائي)

using System.Collections.Concurrent;
using System.Collections.Generic; // لـ HashSet
using System.Linq;

namespace Vitalink.API.Services
{
    public class ConnectionTracker
    {
        // *** الهيكل الجديد: Key (Username) يربط بمجموعة من ConnectionIDs ***
        private static readonly ConcurrentDictionary<string, HashSet<string>> ActiveConnections =
            new ConcurrentDictionary<string, HashSet<string>>();

        /// <summary>
        /// يُضيف معرف الاتصال الجديد إلى مجموعة المستخدم.
        /// </summary>
        public void AddConnection(string username, string connectionId)
        {
            string normalizedUsername = username.ToLower();

            // GetOrAdd للحصول على المجموعة أو إنشائها (HashSet)
            var connections = ActiveConnections.GetOrAdd(
                normalizedUsername,
                _ => new HashSet<string>()
            );

            // إضافة الاتصال الجديد إلى المجموعة
            lock (connections)
            {
                connections.Add(connectionId);
            }
        }

        /// <summary>
        /// يُزيل معرف الاتصال عند انقطاعه.
        /// </summary>
        public void RemoveConnection(string connectionId)
        {
            // نمر على جميع المستخدمين لإزالة الـ ConnectionId
            foreach (var kvp in ActiveConnections)
            {
                lock (kvp.Value)
                {
                    kvp.Value.Remove(connectionId);

                    // إذا أصبحت المجموعة فارغة، يمكن إزالة المستخدم من الـ Dictionary
                    if (kvp.Value.Count == 0)
                    {
                        ActiveConnections.TryRemove(kvp.Key, out _);
                    }
                }
            }
        }

        /// <summary>
        /// الحصول على قائمة بجميع ConnectionIDs النشطة للمستخدم.
        /// </summary>
        public IEnumerable<string> GetConnectionIds(string username)
        {
            string normalizedUsername = username.ToLower();
            if (ActiveConnections.TryGetValue(normalizedUsername, out HashSet<string>? connections))
            {
                // إرجاع قائمة الاتصالات لهذا المستخدم
                return connections.ToList();
            }
            return Enumerable.Empty<string>();
        }
    }
}