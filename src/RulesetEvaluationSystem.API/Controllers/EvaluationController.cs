using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RulesetEvaluationSystem.Application.DTOs.Request;
using RulesetEvaluationSystem.Application.DTOs.Response;
using RulesetEvaluationSystem.Application.Interfaces;

namespace RulesetEvaluationSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EvaluationController : ControllerBase
    {
        private readonly IRuleEvaluationService _evaluationService;
        private readonly ILogger<EvaluationController> _logger;

        public EvaluationController(
            IRuleEvaluationService evaluationService,
            ILogger<EvaluationController> logger)
        {
            _evaluationService = evaluationService;
            _logger = logger;
        }

        
        [HttpPost("evaluate")]
        [ProducesResponseType(typeof(EvaluationResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EvaluationResultDto>> EvaluateOrder([FromBody] OrderDto order)
        {
            if (order == null)
            {
                return BadRequest("Order data is required");
            }

            if (string.IsNullOrWhiteSpace(order.OrderId))
            {
                return BadRequest("OrderId is required");
            }

            _logger.LogInformation("Received evaluation request for Order {OrderId}", order.OrderId);

            var result = await _evaluationService.EvaluateOrderAsync(order);

            if (result.Matched)
            {
                _logger.LogInformation(
                    "Order {OrderId} matched: Plant={Plant}, Ruleset={Ruleset}, Rule={Rule}",
                    order.OrderId, result.ProductionPlant, result.MatchedRuleset, result.MatchedRule);
            }
            else
            {
                _logger.LogWarning("Order {OrderId} did not match any rules", order.OrderId);
            }

            return Ok(result);
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
}
