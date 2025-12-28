using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RulesetEvaluationSystem.Domain.Interfaces;

namespace RulesetEvaluationSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly IEvaluationLogRepository _logRepository;

        public LogsController(IEvaluationLogRepository logRepository)
        {
            _logRepository = logRepository;
        }

       
        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetLogsByOrderId(string orderId)
        {
            var logs = await _logRepository.GetLogsByOrderIdAsync(orderId);
            return Ok(logs);
        }

        
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentLogs([FromQuery] int count = 50)
        {
            var logs = await _logRepository.GetRecentLogsAsync(count);
            return Ok(logs);
        }
    }
}
