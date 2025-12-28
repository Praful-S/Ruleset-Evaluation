using RulesetEvaluationSystem.Application.DTOs.Request;
using RulesetEvaluationSystem.Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace RulesetEvaluationSystem.Application.Interfaces
{
    public interface IRuleEvaluationService
    {
        Task<EvaluationResultDto> EvaluateOrderAsync(OrderDto order);
    }
}
