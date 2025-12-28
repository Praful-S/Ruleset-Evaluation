using RulesetEvaluationSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RulesetEvaluationSystem.Domain.Entities
{
    public class RulesetCondition : BaseEntity
    {
        public int RulesetId { get; set; }
        public string Field { get; set; } = string.Empty;
        public OperatorType Operator { get; set; }
        public string Value { get; set; } = string.Empty;

        // Navigation property
        public Ruleset Ruleset { get; set; } = null!;
    }
}
