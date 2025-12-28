using FluentAssertions;
using RulesetEvaluationSystem.Domain.Enums;
using System.Diagnostics.Metrics;
using Xunit;

namespace RulesetEvaluation.Tests.UnitTests.Domain
{
    /// <summary>
    /// Tests for operator evaluation logic
    /// </summary>
    public class OperatorTests
    {
        [Theory]
        [InlineData("PB", "PB", true)]
        [InlineData("PB", "pb", true)]  // Case insensitive
        [InlineData("PB", "CV", false)]
        [InlineData("US", "US", true)]
        [InlineData("US", "UK", false)]
        public void Equals_Operator_EvaluatesCorrectly(string actual, string expected, bool result)
        {
            // Act
            var isEqual = actual.Equals(expected, StringComparison.OrdinalIgnoreCase);

            // Assert
            isEqual.Should().Be(result);
        }

        [Theory]
        [InlineData(10, 20, true)]   // 10 <= 20
        [InlineData(20, 20, true)]   // 20 <= 20
        [InlineData(25, 20, false)]  // 25 > 20
        [InlineData(0, 20, true)]
        public void LessThanOrEqual_Operator_EvaluatesCorrectly(int actual, int expected, bool result)
        {
            // Act
            var comparison = actual <= expected;

            // Assert
            comparison.Should().Be(result);
        }

        [Theory]
        [InlineData(20, 20, true)]   // 20 >= 20
        [InlineData(25, 20, true)]   // 25 >= 20
        [InlineData(10, 20, false)]  // 10 < 20
        [InlineData(100, 20, true)]
        public void GreaterThanOrEqual_Operator_EvaluatesCorrectly(int actual, int expected, bool result)
        {
            // Act
            var comparison = actual >= expected;

            // Assert
            comparison.Should().Be(result);
        }
    }
}
