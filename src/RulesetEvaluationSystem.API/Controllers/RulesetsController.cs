using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RulesetEvaluationSystem.Domain.Interfaces;

namespace RulesetEvaluationSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RulesetsController : ControllerBase
    {
        private readonly IRulesetRepository _rulesetRepository;
        private readonly ILogger<RulesetsController> _logger;

        public RulesetsController(
            IRulesetRepository rulesetRepository,
            ILogger<RulesetsController> logger)
        {
            _rulesetRepository = rulesetRepository;
            _logger = logger;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllRulesets()
        {
            var rulesets = await _rulesetRepository.GetAllWithRulesAsync();
            return Ok(rulesets);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRuleset(int id)
        {
            var ruleset = await _rulesetRepository.GetRulesetWithConditionsAndRulesAsync(id);

            if (ruleset == null)
            {
                return NotFound($"Ruleset with ID {id} not found");
            }

            return Ok(ruleset);
        }
    }
}
