using Microsoft.AspNetCore.Mvc;
using Vitalink.API.Dtos;
using Vitalink.API.Services;

namespace Vitalink.API.Controllers
{
    // المسار: POST /api/ai/analyze
    [Route("api/ai")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IAiAnalysisService _aiService;

        public AIController(IAiAnalysisService aiService)
        {
            _aiService = aiService;
        }

        public class AnalysisRequest
        {
            // تستقبل مصفوفة البيانات المجمعة من الواجهة الأمامية
            public List<SensorDataDto>? DataBuffer { get; set; }
        }

        /// <summary>
        /// يُنفذ التحليل على بيانات 40 ثانية ويُرجع التنبؤ.
        /// </summary>
        [HttpPost("analyze")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> AnalyzeDataBatch([FromBody] AnalysisRequest request)
        {
            if (request.DataBuffer == null || request.DataBuffer.Count < 30)
            {
                return BadRequest("Data buffer must contain at least 30 samples for analysis.");
            }

            // إرسال المصفوفة إلى خدمة Gemini
            var insight = await _aiService.GeneratePerformanceInsightAsync(request.DataBuffer);

            // إرجاع الجملة الناتجة من Gemini
            return Ok(insight);
        }
    }
}