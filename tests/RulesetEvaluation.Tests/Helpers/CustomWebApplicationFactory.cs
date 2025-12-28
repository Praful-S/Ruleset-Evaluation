using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RulesetEvaluationSystem.Infrastructure.Data;

namespace RulesetEvaluation.Tests.Helpers
{
    /// <summary>
    /// Custom factory for creating test server with in-memory database
    /// </summary>
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the real database registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });

                // Seed test data
                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                context.Database.EnsureCreated();
                SeedTestData(context);
            });
        }

        private void SeedTestData(ApplicationDbContext context)
        {
            // Clear existing data
            context.Rulesets.RemoveRange(context.Rulesets);
            context.SaveChanges();

            // Add test rulesets
            var ruleset = new RulesetEvaluationSystem.Domain.Entities.Ruleset
            {
                Id = 1,
                Name = "Test Ruleset",
                Conditions = new List<RulesetEvaluationSystem.Domain.Entities.RulesetCondition>
                {
                    new() { Field = "PublisherNumber", Operator = RulesetEvaluationSystem.Domain.Enums.OperatorType.Equals, Value = "99999" },
                    new() { Field = "OrderMethod", Operator = RulesetEvaluationSystem.Domain.Enums.OperatorType.Equals, Value = "POD" }
                },
                Rules = new List<RulesetEvaluationSystem.Domain.Entities.Rule>
                {
                    new()
                    {
                        Id = 1,
                        Name = "Test Rule",
                        Priority = 1,
                        ProductionPlant = "US",
                        Conditions = new List<RulesetEvaluationSystem.Domain.Entities.RuleCondition>
                        {
                            new() { Field = "BindTypeCode", Operator = RulesetEvaluationSystem.Domain.Enums.OperatorType.Equals, Value = "PB" },
                            new() { Field = "IsCountry", Operator = RulesetEvaluationSystem.Domain.Enums.OperatorType.Equals, Value = "US" },
                            new() { Field = "PrintQuantity", Operator = RulesetEvaluationSystem.Domain.Enums.OperatorType.LessThanOrEqual, Value = "20" }
                        }
                    }
                }
            };

            context.Rulesets.Add(ruleset);
            context.SaveChanges();
        }
    }
}