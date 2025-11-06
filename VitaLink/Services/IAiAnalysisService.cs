// ملف: Services/IAiAnalysisService.cs

using Vitalink.API.Dtos;

namespace Vitalink.API.Services
{
    public interface IAiAnalysisService
    {
        // الدالة تستقبل مصفوفة البيانات وتُرجع الجملة التحليلية
        Task<string> GeneratePerformanceInsightAsync(List<SensorDataDto> dataBuffer);
    }
}