using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Vitalink.API.Dtos;
using Vitalink.API.Services;
using VitaLink.Models.Data;
using Vitalink.API.Controllers;

namespace Vitalink.API.Hubs
{
    public class SensorDataHub : Hub
    {
        private readonly ConnectionTracker _tracker;
        private readonly VitalinkDbContext _dbContext;
        private readonly AthleteProfilesController _athleteProfilesController;

        public SensorDataHub(ConnectionTracker tracker, VitalinkDbContext dbContext, AthleteProfilesController athleteProfilesController)
        {
            _tracker = tracker;
            _dbContext = dbContext;
            _athleteProfilesController = athleteProfilesController;
        }

        // Called when a new connection registers (maps username to connection ID)
        public async Task RegisterConnection(string username)
        {
            _tracker.AddConnection(username, Context.ConnectionId);
            Debug.WriteLine($"[CONNECTION] User {username} registered ID: {Context.ConnectionId}");
        }

        // Receives live sensor data from a device
        public async Task SendSensorData(SensorDataDto data)
        {
            var incomingBeltId = data.BeltID;

            // Find athlete by BeltID
            var targetUsername = await _dbContext.AthleteProfiles
                .Where(a => a.BeltID == incomingBeltId)
                .Select(a => a.FirstName) // make sure you use username here
                .FirstOrDefaultAsync();

            if (targetUsername == null)
            {
                Debug.WriteLine($"[WARNING] Unknown BeltID: {incomingBeltId}");
                return;
            }

            try
            {
                // Save the sensor data first
                await _athleteProfilesController.RawData(data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Failed to save data: {ex.Message}");
                return; // do not broadcast unsaved data
            }

            // Broadcast to connected clients
            var targetConnectionIds = _tracker.GetConnectionIds(targetUsername);

            if (targetConnectionIds.Any())
            {
                await Clients.Clients(targetConnectionIds.ToList())
                    .SendAsync("ReceiveLiveUpdate", data);

                Debug.WriteLine($"[STREAM SUCCESS] Data sent to {targetConnectionIds.Count()} connection(s) for {targetUsername}.");
            }
            else
            {
                Debug.WriteLine($"[WARNING] No active connections for {targetUsername}");
            }
        }

        // Handles disconnection cleanup
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _tracker.RemoveConnection(Context.ConnectionId);
            Debug.WriteLine($"[DISCONNECT] Connection ID {Context.ConnectionId} removed.");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
