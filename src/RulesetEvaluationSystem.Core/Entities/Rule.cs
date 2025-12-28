using System;
using System.Collections.Generic;
using System.Text;

namespace RulesetEvaluationSystem.Domain.Entities
{
    public class Rule : BaseEntity
    {
        public int RulesetId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Priority { get; set; } = 0; // Lower number = higher priority

        // Result when this rule matches
        public string ProductionPlant { get; set; } = string.Empty;

        // Navigation properties
        public Ruleset Ruleset { get; set; } = null!;
        public ICollection<RuleCondition> Conditions { get; set; } = new List<RuleCondition>();
    }
}
