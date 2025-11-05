using Microsoft.AspNetCore.SignalR;
using Vitalink.API.Dtos;
using System.Diagnostics; // للحصول على Debug.WriteLine

namespace Vitalink.API.Hubs
{
    /// <summary>
    /// مركز بيانات الحساسات (Hub)
    /// مسؤول عن استقبال بيانات WebSockets من المتحكمات الدقيقة وبثها للواجهة الأمامية.
    /// </summary>
    public class SensorDataHub : Hub
    {

        public async Task SendSensorData(SensorDataDto data)
        {
            // ----------------------------------------------------
            // 1. التحقق من وجود البيانات الأساسية (Presence Check)
            // ----------------------------------------------------
            // HeartRate هو الحقل الوحيد المحدد كـ [Required] في الـ DTO الخاص بك.


            // ----------------------------------------------------
            // 2. طباعة جميع البيانات في الكونسول (للتأكد من وصولها كاملاً)
            // ----------------------------------------------------

            Debug.WriteLine("================ VITALINK SENSOR DATA RECEIVED ================");
            // تم حذف AthleteID و Timestamp من الطباعة لأنها غير موجودة حالياً في الـ DTO

            Debug.WriteLine($"- Heart Rate (HR): {data.HeartRate:F1} BPM");
            Debug.WriteLine($"- Oxygen Saturation (SpO2): {data.Spo2}%");
            Debug.WriteLine($"- Temperature (Temp): {data.Temperature:F2}°C");

            Debug.WriteLine($"- Motion (Acc): X={data.AccX:F2}, Y={data.AccY:F2}, Z={data.AccZ:F2}");
            Debug.WriteLine($"- Sweat Level: {data.Sweat}");
            Debug.WriteLine("===============================================================");

            // ----------------------------------------------------
            // 3. البث الفوري للعملاء (الواجهة الأمامية)
            // ----------------------------------------------------
            // "ReceiveLiveUpdate" هو اسم الدالة التي يستمع إليها Frontend
            await Clients.All.SendAsync("ReceiveLiveUpdate", data);
        }

        /// <summary>
        /// يُستدعى عند إنشاء اتصال WebSocket جديد
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            Debug.WriteLine($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// يُستدعى عند انقطاع اتصال WebSocket
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Debug.WriteLine($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}