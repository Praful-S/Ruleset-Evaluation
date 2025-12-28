using RulesetEvaluationSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RulesetEvaluationSystem.Domain.Interfaces
{
    public interface IRulesetRepository : IRepository<Ruleset>
    {
        Task<IEnumerable<Ruleset>> GetAllWithRulesAsync();
        Task<Ruleset?> GetRulesetWithConditionsAndRulesAsync(int id);
    }
}
