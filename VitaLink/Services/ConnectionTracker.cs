

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Vitalink.API.Services
{
    public class ConnectionTracker
    {
        private static readonly ConcurrentDictionary<string, HashSet<string>> ActiveConnections =
            new ConcurrentDictionary<string, HashSet<string>>();

        public void AddConnection(string username, string connectionId)
        {
            string normalizedUsername = username.ToLower();

            var connections = ActiveConnections.GetOrAdd(
                normalizedUsername,
                _ => new HashSet<string>()
            );

            lock (connections)
            {
                connections.Add(connectionId);
            }
        }

        public void RemoveConnection(string connectionId)
        {
            foreach (var kvp in ActiveConnections)
            {
                lock (kvp.Value)
                {
                    kvp.Value.Remove(connectionId);

                    if (kvp.Value.Count == 0)
                    {
                        ActiveConnections.TryRemove(kvp.Key, out _);
                    }
                }
            }
        }

        public IEnumerable<string> GetConnectionIds(string username)
        {
            string normalizedUsername = username.ToLower();
            if (ActiveConnections.TryGetValue(normalizedUsername, out HashSet<string>? connections))
            {
                return connections.ToList();
            }
            return Enumerable.Empty<string>();
        }
    }
}