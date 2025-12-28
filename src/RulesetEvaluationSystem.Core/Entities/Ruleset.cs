using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace RulesetEvaluationSystem.Domain.Entities
{
    public class Ruleset : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Navigation properties
        public ICollection<RulesetCondition> Conditions { get; set; } = new List<RulesetCondition>();
        public ICollection<Rule> Rules { get; set; } = new List<Rule>();
    }
}
