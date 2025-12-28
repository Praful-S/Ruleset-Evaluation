using Microsoft.EntityFrameworkCore;
using RulesetEvaluationSystem.Domain.Entities;
using RulesetEvaluationSystem.Domain.Interfaces;
using RulesetEvaluationSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace RulesetEvaluationSystem.Infrastructure.Repositories
{
    public class EvaluationLogRepository : Repository<EvaluationLog>, IEvaluationLogRepository
    {
        public EvaluationLogRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<EvaluationLog>> GetLogsByOrderIdAsync(string orderId)
        {
            return await _context.EvaluationLogs
                .Where(log => log.OrderId == orderId)
                .OrderByDescending(log => log.EvaluatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<EvaluationLog>> GetRecentLogsAsync(int count)
        {
            return await _context.EvaluationLogs
                .OrderByDescending(log => log.EvaluatedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}
