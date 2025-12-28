using System;
using System.Collections.Generic;
using System.Text;

namespace RulesetEvaluationSystem.Application.DTOs.Response
{
    public class EvaluationResultDto
    {
        public bool Matched { get; set; }
        public string? ProductionPlant { get; set; }
        public string? MatchedRuleset { get; set; }
        public string? MatchedRule { get; set; }
        public string? Reason { get; set; }
        public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
    }
}
