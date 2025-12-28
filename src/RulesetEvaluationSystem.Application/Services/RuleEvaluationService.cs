using Microsoft.Extensions.Logging;
using RulesetEvaluationSystem.Application.DTOs.Request;
using RulesetEvaluationSystem.Application.DTOs.Response;
using RulesetEvaluationSystem.Application.Interfaces;
using RulesetEvaluationSystem.Domain.Entities;
using RulesetEvaluationSystem.Domain.Enums;
using RulesetEvaluationSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace RulesetEvaluationSystem.Application.Services
{
    public class RuleEvaluationService : IRuleEvaluationService
    {
        private readonly IRulesetRepository _rulesetRepository;
        private readonly IEvaluationLogRepository _logRepository;
        private readonly ILogger<RuleEvaluationService> _logger;

        public RuleEvaluationService(
            IRulesetRepository rulesetRepository,
            IEvaluationLogRepository logRepository,
            ILogger<RuleEvaluationService> logger)
        {
            _rulesetRepository = rulesetRepository;
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task<EvaluationResultDto> EvaluateOrderAsync(OrderDto order)
        {
            _logger.LogInformation("Starting evaluation for Order {OrderId}", order.OrderId);

            try
            {
                // Step 1: Load all rulesets with their conditions and rules
                var rulesets = await _rulesetRepository.GetAllWithRulesAsync();

                _logger.LogInformation("Loaded {Count} rulesets", rulesets.Count());

                // Step 2: Find the matching ruleset
                var matchedRuleset = FindMatchingRuleset(order, rulesets);

                if (matchedRuleset == null)
                {
                    _logger.LogWarning("No matching ruleset found for Order {OrderId}", order.OrderId);
                    return await LogAndReturnResult(order, false, null, null, null, "No matching ruleset found");
                }

                _logger.LogInformation("Matched Ruleset: {RulesetName} with {RuleCount} rules",
                    matchedRuleset.Name, matchedRuleset.Rules.Count);

                // Step 3: Evaluate rules within the matched ruleset
                var orderedRules = matchedRuleset.Rules.OrderBy(r => r.Priority).ToList();

                foreach (var rule in orderedRules)
                {
                    _logger.LogInformation("Evaluating Rule: {RuleName} (Priority: {Priority})",
                        rule.Name, rule.Priority);

                    if (EvaluateRule(order, rule))
                    {
                        _logger.LogInformation("✓ Rule matched: {RuleName} → Plant: {Plant}",
                            rule.Name, rule.ProductionPlant);

                        var reason = BuildReason(rule);
                        return await LogAndReturnResult(
                            order,
                            true,
                            rule.ProductionPlant,
                            matchedRuleset.Name,
                            rule.Name,
                            reason);
                    }
                    else
                    {
                        _logger.LogInformation("✗ Rule did not match: {RuleName}", rule.Name);
                    }
                }

                // No rule matched within the ruleset
                _logger.LogWarning("No rule conditions satisfied for Order {OrderId} in Ruleset {RulesetName}",
                    order.OrderId, matchedRuleset.Name);

                return await LogAndReturnResult(
                    order,
                    false,
                    null,
                    matchedRuleset.Name,
                    null,
                    "No rule conditions satisfied");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating Order {OrderId}", order.OrderId);
                throw;
            }
        }

        /// <summary>
        /// Finds the first ruleset that matches the order's top-level attributes
        /// </summary>
        private Ruleset? FindMatchingRuleset(OrderDto order, IEnumerable<Ruleset> rulesets)
        {
            foreach (var ruleset in rulesets)
            {
                _logger.LogInformation("Checking Ruleset: {RulesetName}", ruleset.Name);

                if (EvaluateRulesetConditions(order, ruleset.Conditions))
                {
                    _logger.LogInformation("✓ Ruleset matched: {RulesetName}", ruleset.Name);
                    return ruleset;
                }
                else
                {
                    _logger.LogInformation("✗ Ruleset did not match: {RulesetName}", ruleset.Name);
                }
            }
            return null;
        }

        private bool EvaluateRulesetConditions(OrderDto order, ICollection<RulesetCondition> conditions)
        {
            foreach (var condition in conditions)
            {
                var orderValue = ExtractOrderValue(order, condition.Field);
                var result = EvaluateCondition(orderValue, condition.Operator, condition.Value);

                _logger.LogDebug("  Condition: {Field} {Operator} {ExpectedValue} | Actual: {ActualValue} | Result: {Result}",
                    condition.Field, condition.Operator, condition.Value, orderValue ?? "null", result);

                if (!result)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if all rule conditions are satisfied
        /// </summary>
        private bool EvaluateRule(OrderDto order, Rule rule)
        {
            _logger.LogDebug("  Evaluating {Count} conditions for Rule: {RuleName}",
                rule.Conditions.Count, rule.Name);

            foreach (var condition in rule.Conditions)
            {
                var orderValue = ExtractOrderValue(order, condition.Field);
                var result = EvaluateCondition(orderValue, condition.Operator, condition.Value);

                _logger.LogDebug("    Condition: {Field} {Operator} {ExpectedValue} | Actual: {ActualValue} | Result: {Result}",
                    condition.Field, condition.Operator, condition.Value, orderValue ?? "null", result);

                if (!result)
                {
                    _logger.LogDebug("    ✗ Rule {RuleName} failed at condition: {Field}",
                        rule.Name, condition.Field);
                    return false;
                }
            }

            _logger.LogDebug("  ✓ All conditions satisfied for Rule: {RuleName}", rule.Name);
            return true;
        }

        /// <summary>
        /// Extracts a value from the order based on the field name
        /// </summary>
        private string? ExtractOrderValue(OrderDto order, string field)
        {
            var value = field switch
            {
                "PublisherNumber" => order.PublisherNumber,
                "OrderMethod" => order.OrderMethod,
                "IsCountry" => order.Shipments?.FirstOrDefault()?.ShipTo?.IsoCountry,
                "BindTypeCode" => GetBindTypeCode(order),
                "PrintQuantity" => order.Items?.FirstOrDefault()?.PrintQuantity.ToString(),
                _ => null
            };

            _logger.LogDebug("    ExtractOrderValue({Field}) = {Value}", field, value ?? "null");
            return value;
        }

        /// <summary>
        /// Gets the BindTypeCode from the order's components
        /// Checks all components and returns the first BindTypeCode found
        /// </summary>
        private string? GetBindTypeCode(OrderDto order)
        {
            if (order.Items == null || !order.Items.Any())
            {
                _logger.LogDebug("    No items in order");
                return null;
            }

            foreach (var item in order.Items)
            {
                if (item.Components == null || !item.Components.Any())
                {
                    _logger.LogDebug("    No components in item {Sku}", item.Sku);
                    continue;
                }

                foreach (var component in item.Components)
                {
                    if (component.Attributes != null &&
                        component.Attributes.TryGetValue("BindTypeCode", out var bindType))
                    {
                        _logger.LogDebug("    Found BindTypeCode: {BindType} in component {Code}",
                            bindType, component.Code);
                        return bindType;
                    }
                }
            }

            _logger.LogDebug("    No BindTypeCode found in any component");
            return null;
        }

        /// <summary>
        /// Evaluates a single condition using the specified operator
        /// </summary>
        private bool EvaluateCondition(string? actualValue, OperatorType op, string expectedValue)
        {
            if (actualValue == null)
            {
                _logger.LogDebug("      Condition failed: actualValue is null");
                return false;
            }

            var result = op switch
            {
                OperatorType.Equals => actualValue.Equals(expectedValue, StringComparison.OrdinalIgnoreCase),
                OperatorType.NotEquals => !actualValue.Equals(expectedValue, StringComparison.OrdinalIgnoreCase),
                OperatorType.LessThanOrEqual => CompareNumeric(actualValue, expectedValue) <= 0,
                OperatorType.GreaterThanOrEqual => CompareNumeric(actualValue, expectedValue) >= 0,
                OperatorType.LessThan => CompareNumeric(actualValue, expectedValue) < 0,
                OperatorType.GreaterThan => CompareNumeric(actualValue, expectedValue) > 0,
                _ => false
            };

            _logger.LogDebug("      Comparison: {Actual} {Operator} {Expected} = {Result}",
                actualValue, op, expectedValue, result);

            return result;
        }

        /// <summary>
        /// Compares two values numerically
        /// </summary>
        private int CompareNumeric(string actual, string expected)
        {
            if (decimal.TryParse(actual, out var actualNum) &&
                decimal.TryParse(expected, out var expectedNum))
            {
                return actualNum.CompareTo(expectedNum);
            }
            return string.Compare(actual, expected, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Builds a human-readable reason for the match
        /// </summary>
        private string BuildReason(Rule rule)
        {
            var conditions = rule.Conditions
                .Select(c => $"{c.Field}{GetOperatorSymbol(c.Operator)}{c.Value}")
                .ToList();
            return string.Join(", ", conditions);
        }

        private string GetOperatorSymbol(OperatorType op)
        {
            return op switch
            {
                OperatorType.Equals => "=",
                OperatorType.NotEquals => "!=",
                OperatorType.LessThanOrEqual => "<=",
                OperatorType.GreaterThanOrEqual => ">=",
                OperatorType.LessThan => "<",
                OperatorType.GreaterThan => ">",
                _ => "?"
            };
        }

        /// <summary>
        /// Logs the evaluation result and returns the DTO
        /// </summary>
        private async Task<EvaluationResultDto> LogAndReturnResult(
            OrderDto order,
            bool matched,
            string? plant,
            string? ruleset,
            string? rule,
            string? reason)
        {
            var log = new EvaluationLog
            {
                OrderId = order.OrderId,
                InputJson = JsonSerializer.Serialize(order),
                Matched = matched,
                MatchedRulesetName = ruleset,
                MatchedRuleName = rule,
                ProductionPlant = plant,
                Reason = reason,
                EvaluatedAt = DateTime.UtcNow
            };

            await _logRepository.AddAsync(log);

            return new EvaluationResultDto
            {
                Matched = matched,
                ProductionPlant = plant,
                MatchedRuleset = ruleset,
                MatchedRule = rule,
                Reason = reason
            };
        }
    }
}
