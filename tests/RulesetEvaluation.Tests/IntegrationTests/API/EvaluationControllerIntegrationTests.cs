using FluentAssertions;
using RulesetEvaluationSystem.Application.DTOs.Request;
using RulesetEvaluationSystem.Application.DTOs.Response;
using RulesetEvaluation.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace RulesetEvaluation.Tests.IntegrationTests.API
{
    /// <summary>
    /// Integration tests for Evaluation API endpoints
    /// Tests the full stack: API → Service → Database
    /// </summary>
    public class EvaluationControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public EvaluationControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task EvaluateOrder_ValidRequest_ReturnsOkWithResult()
        {
            // Arrange
            var order = new OrderDto
            {
                OrderId = "INT-TEST-001",
                PublisherNumber = "99999",
                PublisherName = "Test Publisher",
                OrderMethod = "POD",
                Shipments = new List<ShipmentDto>
                {
                    new() { ShipTo = new ShipToDto { IsoCountry = "US" } }
                },
                Items = new List<ItemDto>
                {
                    new()
                    {
                        Sku = "TEST-SKU",
                        PrintQuantity = 10,
                        Components = new List<ComponentDto>
                        {
                            new()
                            {
                                Code = "Cover",
                                Attributes = new Dictionary<string, string>
                                {
                                    { "BindTypeCode", "PB" }
                                }
                            }
                        }
                    }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/evaluation/evaluate", order);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<EvaluationResultDto>();
            result.Should().NotBeNull();
            result!.Matched.Should().BeTrue();
            result.ProductionPlant.Should().Be("US");
        }

        [Fact]
        public async Task EvaluateOrder_MissingOrderId_ReturnsBadRequest()
        {
            // Arrange
            var order = new OrderDto
            {
                OrderId = "", // Invalid: empty
                PublisherNumber = "99999",
                OrderMethod = "POD"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/evaluation/evaluate", order);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task EvaluateOrder_NullOrder_ReturnsBadRequest()
        {
            // Act
            var response = await _client.PostAsJsonAsync("/api/evaluation/evaluate", (OrderDto?)null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task HealthCheck_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/evaluation/health");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("healthy");
        }

        [Fact]
        public async Task GetAllRulesets_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/rulesets");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetRuleset_ValidId_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/rulesets/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetRuleset_InvalidId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/rulesets/9999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
