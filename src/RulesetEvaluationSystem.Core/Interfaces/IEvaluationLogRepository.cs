using RulesetEvaluationSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RulesetEvaluationSystem.Domain.Interfaces
{
    public interface IEvaluationLogRepository : IRepository<EvaluationLog>
    {
        Task<IEnumerable<EvaluationLog>> GetLogsByOrderIdAsync(string orderId);
        Task<IEnumerable<EvaluationLog>> GetRecentLogsAsync(int count);
    }
}
