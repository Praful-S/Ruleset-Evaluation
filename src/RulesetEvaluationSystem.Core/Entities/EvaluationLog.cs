using System;
using System.Collections.Generic;
using System.Text;

namespace RulesetEvaluationSystem.Domain.Entities
{
    public class EvaluationLog : BaseEntity
    {
        public string OrderId { get; set; } = string.Empty;
        public string InputJson { get; set; } = string.Empty;

        public bool Matched { get; set; }
        public string? MatchedRulesetName { get; set; }
        public string? MatchedRuleName { get; set; }
        public string? ProductionPlant { get; set; }

        public string? Reason { get; set; }
        public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
    }
}
