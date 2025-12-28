using Microsoft.Extensions.DependencyInjection;
using RulesetEvaluationSystem.Application.Interfaces;
using RulesetEvaluationSystem.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace RulesetEvaluationSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Services
            services.AddScoped<IRuleEvaluationService, RuleEvaluationService>();

            return services;
        }
    }
}
