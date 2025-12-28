using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RulesetEvaluationSystem.Domain.Interfaces;
using RulesetEvaluationSystem.Infrastructure.Data;
using RulesetEvaluationSystem.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace RulesetEvaluationSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Database Context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IRulesetRepository, RulesetRepository>();
            services.AddScoped<IEvaluationLogRepository, EvaluationLogRepository>();

            return services;
        }
    }
}
