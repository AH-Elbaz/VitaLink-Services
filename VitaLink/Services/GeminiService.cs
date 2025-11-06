// ملف: Services/GeminiService.cs

using Google.Cloud.AI.GenerativeAI;
using Vitalink.API.Dtos;
using System.Text.Json; // لسهولة تحويل البيانات إلى نص

namespace Vitalink.API.Services
{
    public class GeminiService : IAiAnalysisService
    {
        private readonly GenerativeModel _model;
        private const string MODEL_NAME = "gemini-2.5-flash"; // نموذج مناسب للمهام السريعة

        public GeminiService(IConfiguration configuration)
        {
            // التهيئة: يحصل على المفتاح من إعدادات التطبيق (appsettings.json أو Azure Settings)
            var apiKey = configuration["Gemini:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("Gemini API Key is not configured.");
            }

            var client = new GeminiClient(apiKey);
            _model = client.GenerativeModel(MODEL_NAME);
        }

        public async Task<string> GeneratePerformanceInsightAsync(List<SensorDataDto> dataBuffer)
        {
            // 1. تحويل بيانات الحساسات إلى نص JSON
            var dataJsonString = JsonSerializer.Serialize(dataBuffer);

            // 2. بناء الـ Prompt للنموذج
            var systemPrompt = $@"
                You are VITALINK's highly specialized Sports Data Analyst AI. 
                Your task is to analyze the following 40 seconds of time-series biometric data. 
                Based ONLY on the data, output a SINGLE, brief, professional, and actionable performance insight or fatigue prediction (maximum 15 words).

                CONSTRAINTS:
                - Focus on Heart Rate, SpO2, and AccelZ trends.
                - If HR trends upwards sharply (>160 BPM), predict 'Strain/Fatigue'.
                - If HR is stable and SpO2 is high (>97%), predict 'Optimal Performance'.
                - DO NOT include the data in your final output.
                
                --- RAW DATA ARRAY (40 Seconds) ---
                {dataJsonString}
                ";

            // 3. إرسال الطلب إلى Gemini API
            var response = await _model.GenerateContentAsync(systemPrompt);

            // 4. إرجاع النص الناتج
            return response.Text;
        }
    }
}