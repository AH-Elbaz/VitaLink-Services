   
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using System.Diagnostics;
    using System.Security.Claims;
    using Vitalink.API.Dtos;
    using Vitalink.API.Services;
    using VitaLink.Models.Data;

    namespace Vitalink.API.Hubs
    {
        public class SensorDataHub : Hub
        {
            private readonly ConnectionTracker _tracker;
            private readonly VitalinkDbContext _dbContext; 

            
            public SensorDataHub(ConnectionTracker tracker, VitalinkDbContext dbContext)
            {
                _tracker = tracker;
                _dbContext = dbContext;
            }

         

            public async Task RegisterConnection(string username)
            {
                
                _tracker.AddConnection(username, Context.ConnectionId);
                Debug.WriteLine($"[CONNECTION] User {username} registered ID: {Context.ConnectionId}");
            }

            
            public async Task SendSensorData(SensorDataDto data)
            {
             
              
                var incomingBeltId = data.BeltID;

              
                var targetUsername = await _dbContext.AthleteProfiles
                                              .Where(a => a.BeltID == incomingBeltId)
                                              .Select(a => a.FirstName) 
                                              .FirstOrDefaultAsync();

                if (targetUsername != null)
                {
             
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
