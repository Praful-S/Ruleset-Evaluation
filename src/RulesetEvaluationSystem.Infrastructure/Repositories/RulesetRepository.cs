using Microsoft.EntityFrameworkCore;
using RulesetEvaluationSystem.Domain.Entities;
using RulesetEvaluationSystem.Domain.Interfaces;
using RulesetEvaluationSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace RulesetEvaluationSystem.Infrastructure.Repositories
{
    public class RulesetRepository : Repository<Ruleset>, IRulesetRepository
    {
        public RulesetRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Ruleset>> GetAllWithRulesAsync()
        {
            return await _context.Rulesets
                .Include(r => r.Conditions)  // Load ruleset conditions
                .Include(r => r.Rules)       // Load rules
                    .ThenInclude(rule => rule.Conditions)  // Load rule conditions
                .Where(r => r.IsActive)
                .ToListAsync();
        }

        public async Task<Ruleset?> GetRulesetWithConditionsAndRulesAsync(int id)
        {
            return await _context.Rulesets
                .Include(r => r.Conditions)
                .Include(r => r.Rules)
                    .ThenInclude(rule => rule.Conditions)
                .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);
        }
    }
}
