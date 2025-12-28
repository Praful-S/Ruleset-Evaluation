using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RulesetEvaluationSystem.Application.DTOs.Request;
using RulesetEvaluationSystem.Application.Services;
using RulesetEvaluationSystem.Domain.Entities;
using RulesetEvaluationSystem.Domain.Enums;
using RulesetEvaluationSystem.Domain.Interfaces;
using Xunit;

namespace RulesetEvaluation.Tests.UnitTests.Services
{
    public class RuleEvaluationServiceTests
    {
        private readonly Mock<IRulesetRepository> _mockRulesetRepo;
        private readonly Mock<IEvaluationLogRepository> _mockLogRepo;
        private readonly Mock<ILogger<RuleEvaluationService>> _mockLogger;
        private readonly RuleEvaluationService _service;

        public RuleEvaluationServiceTests()
        {
            _mockRulesetRepo = new Mock<IRulesetRepository>();
            _mockLogRepo = new Mock<IEvaluationLogRepository>();
            _mockLogger = new Mock<ILogger<RuleEvaluationService>>();

            _service = new RuleEvaluationService(
                _mockRulesetRepo.Object,
                _mockLogRepo.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task EvaluateOrder_WithMatchingRule_ReturnsCorrectPlant()
        {
            // Arrange
            var order = CreateTestOrder("99999", "POD", "US", "PB", 10);
            var rulesets = CreateTestRulesets();

            _mockRulesetRepo.Setup(r => r.GetAllWithRulesAsync())
                .ReturnsAsync(rulesets);

            _mockLogRepo.Setup(r => r.AddAsync(It.IsAny<EvaluationLog>()))
                .ReturnsAsync((EvaluationLog log) => log);

            // Act
            var result = await _service.EvaluateOrderAsync(order);

            // Assert
            result.Should().NotBeNull();
            result.Matched.Should().BeTrue();
            result.ProductionPlant.Should().Be("US");
            result.MatchedRuleset.Should().Be("Ruleset Two");
        }

        [Fact]
        public async Task EvaluateOrder_WithNoMatchingRuleset_ReturnsNotMatched()
        {
            // Arrange
            var order = CreateTestOrder("88888", "POD", "US", "PB", 10);
            var rulesets = CreateTestRulesets();

            _mockRulesetRepo.Setup(r => r.GetAllWithRulesAsync())
                .ReturnsAsync(rulesets);

            _mockLogRepo.Setup(r => r.AddAsync(It.IsAny<EvaluationLog>()))
                .ReturnsAsync((EvaluationLog log) => log);

            // Act
            var result = await _service.EvaluateOrderAsync(order);

            // Assert
            result.Should().NotBeNull();
            result.Matched.Should().BeFalse();
            result.Reason.Should().Contain("No matching ruleset");
        }

        [Theory]
        [InlineData(5, true)]   // 5 <= 20
        [InlineData(20, true)]  // 20 <= 20
        [InlineData(25, false)] // 25 > 20
        public async Task EvaluateOrder_WithDifferentQuantities_EvaluatesCorrectly(
            int quantity,
            bool shouldMatch)
        {
            // Arrange
            var order = CreateTestOrder("99999", "POD", "US", "PB", quantity);
            var rulesets = CreateTestRulesets();

            _mockRulesetRepo.Setup(r => r.GetAllWithRulesAsync())
                .ReturnsAsync(rulesets);

            _mockLogRepo.Setup(r => r.AddAsync(It.IsAny<EvaluationLog>()))
                .ReturnsAsync((EvaluationLog log) => log);

            // Act
            var result = await _service.EvaluateOrderAsync(order);

            // Assert
            result.Matched.Should().Be(shouldMatch);
        }

        private OrderDto CreateTestOrder(
            string publisherNumber,
            string orderMethod,
            string country,
            string bindType,
            int quantity)
        {
            return new OrderDto
            {
                OrderId = "TEST-001",
                PublisherNumber = publisherNumber,
                OrderMethod = orderMethod,
                Shipments = new List<ShipmentDto>
                {
                    new ShipmentDto
                    {
                        ShipTo = new ShipToDto { IsoCountry = country }
                    }
                },
                Items = new List<ItemDto>
                {
                    new ItemDto
                    {
                        PrintQuantity = quantity,
                        Components = new List<ComponentDto>
                        {
                            new ComponentDto
                            {
                                Attributes = new Dictionary<string, string>
                                {
                                    { "BindTypeCode", bindType }
                                }
                            }
                        }
                    }
                }
            };
        }

        private List<Ruleset> CreateTestRulesets()
        {
            var ruleset = new Ruleset
            {
                Id = 2,
                Name = "Ruleset Two",
                Conditions = new List<RulesetCondition>
                {
                    new RulesetCondition
                    {
                        Field = "PublisherNumber",
                        Operator = OperatorType.Equals,
                        Value = "99999"
                    },
                    new RulesetCondition
                    {
                        Field = "OrderMethod",
                        Operator = OperatorType.Equals,
                        Value = "POD"
                    }
                },
                Rules = new List<Rule>
                {
                    new Rule
                    {
                        Id = 3,
                        Name = "Rule 3",
                        Priority = 1,
                        ProductionPlant = "US",
                        Conditions = new List<RuleCondition>
                        {
                            new RuleCondition
                            {
                                Field = "BindTypeCode",
                                Operator = OperatorType.Equals,
                                Value = "PB"
                            },
                            new RuleCondition
                            {
                                Field = "IsCountry",
                                Operator = OperatorType.Equals,
                                Value = "US"
                            },
                            new RuleCondition
                            {
                                Field = "PrintQuantity",
                                Operator = OperatorType.LessThanOrEqual,
                                Value = "20"
                            }
                        }
                    }
                }
            };

            return new List<Ruleset> { ruleset };
        }
    }
}