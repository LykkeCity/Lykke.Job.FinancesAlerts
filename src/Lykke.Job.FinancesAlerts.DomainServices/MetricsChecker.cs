using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.FinancesAlerts.Client.Models;
using Lykke.Job.FinancesAlerts.Domain;
using Lykke.Job.FinancesAlerts.Domain.Repositories;
using Lykke.Job.FinancesAlerts.Domain.Services;
using MoreLinq;

namespace Lykke.Job.FinancesAlerts.DomainServices
{
    public class MetricsChecker : IMetricsChecker
    {
        private readonly ILog _log;
        private readonly IAlertRuleRepository _alertRuleRepository;
        private readonly IAlertSubscriptionRepository _alertSubscriptionRepository;
        private readonly IMetricCalculatorRegistry _metricCalculatorRegistry;
        private readonly IAlertNotifier _alertNotifier;
        private readonly Dictionary<string, DateTime> _lastAlertTimeDict = new Dictionary<string, DateTime>();
        private readonly HashSet<string> _activeAlerts = new HashSet<string>();
        private readonly HashSet<string> _disabledMetrics = new HashSet<string>();

        public MetricsChecker(
            ILogFactory logFactory,
            IAlertRuleRepository alertRuleRepository,
            IAlertSubscriptionRepository alertSubscriptionRepository,
            IMetricCalculatorRegistry metricCalculatorRegistry,
            IAlertNotifier alertNotifier,
            string[] disabledMetrics)
        {
            _log = logFactory.CreateLog(this);
            _alertRuleRepository = alertRuleRepository;
            _alertSubscriptionRepository = alertSubscriptionRepository;
            _metricCalculatorRegistry = metricCalculatorRegistry;
            _alertNotifier = alertNotifier;
            disabledMetrics?.ForEach(m => _disabledMetrics.Add(m));
        }

        public async Task CheckAllMetricsAsync()
        {
            var metricCalculators = _metricCalculatorRegistry.GetAllMetricCalculators();

            await Task.WhenAll(
                metricCalculators
                    .Where(m => !_disabledMetrics.Contains(m.MetricInfo.Name))
                    .Select(CheckMetricAsync));
        }

        private async Task CheckMetricAsync(IMetricCalculator metricCalculator)
        {
            try
            {
                var metricInfo = metricCalculator.MetricInfo;

                var calculationTask = metricCalculator.CalculateMetricsAsync();
                var rulesFetchingTask = _alertRuleRepository.GetByMetricAsync(metricInfo.Name);

                await Task.WhenAll(calculationTask, rulesFetchingTask);

                var alertRules = rulesFetchingTask.Result;
                var metrics = calculationTask.Result;

                foreach (var metric in metrics)
                {
                    foreach (var alertRule in alertRules)
                    {
                        bool needToFireAlert;
                        switch (alertRule.ComparisonType)
                        {
                            case ComparisonType.GreaterThan:
                                needToFireAlert = metric.Value > alertRule.ThresholdValue;
                                break;
                            case ComparisonType.GreaterOrEqual:
                                needToFireAlert = metric.Value >= alertRule.ThresholdValue;
                                break;
                            case ComparisonType.Equal:
                                needToFireAlert = metric.Value == alertRule.ThresholdValue;
                                break;
                            case ComparisonType.LessThan:
                                needToFireAlert = metric.Value < alertRule.ThresholdValue;
                                break;
                            case ComparisonType.LessOrEqual:
                                needToFireAlert = metric.Value <= alertRule.ThresholdValue;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        if (needToFireAlert)
                        {
                            await ProcessAlertEventAsync(
                                    metric,
                                    alertRule,
                                    true)
                                ;
                        }
                        else
                        {
                            var activeMetricRuleKey = GenerateActiveMetricRuleKey(metric, alertRule);
                            if (_activeAlerts.Contains(activeMetricRuleKey))
                            {
                                await ProcessAlertEventAsync(
                                        metric,
                                        alertRule,
                                        false)
                                    ;
                                _activeAlerts.Remove(activeMetricRuleKey);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
        }

        private async Task ProcessAlertEventAsync(
            Metric metric,
            IAlertRule alertRule,
            bool isStarted)
        {
            var subscriptions = await _alertSubscriptionRepository.GetByAlertRuleAsync(alertRule.Id);
            if (!subscriptions.Any())
                return;

            var activeMetricRuleKey = GenerateActiveMetricRuleKey(metric, alertRule);
            _activeAlerts.Add(activeMetricRuleKey);

            var message = GenerateAlerMessage(
                metric,
                alertRule,
                isStarted);
            foreach (var subscription in subscriptions)
            {
                var alertSubscriptionKey = GenerateActiveSubscriptionKey(metric, subscription, isStarted);
                var now = DateTime.UtcNow;
                if (_lastAlertTimeDict.TryGetValue(alertSubscriptionKey, out var lastAlertTime))
                {
                    if (now - lastAlertTime < subscription.AlertFrequency)
                        continue;
                }

                _lastAlertTimeDict[alertSubscriptionKey] = now;

                await _alertNotifier.NotifyAsync(
                    subscription.Type,
                    subscription.Address,
                    isStarted ? $"{metric.Name} alert" : $"{metric.Name} alerting is off",
                    message)
                    ;
            }
        }

        private string GenerateActiveMetricRuleKey(Metric metric, IAlertRule alertRule)
        {
            return $"{alertRule.Id}_{metric.Instrument}";
        }

        private string GenerateActiveSubscriptionKey(Metric metric, IAlertSubscription subscription, bool isStarted)
        {
            return $"{metric.Name}_{metric.Instrument}_{subscription.Type}_{isStarted}";
        }

        private string GenerateAlerMessage(
            Metric metric,
            IAlertRule alertRule,
            bool isStarted)
        {
            string op;
            switch (alertRule.ComparisonType)
            {
                case ComparisonType.GreaterThan:
                    op = "greater than";
                    break;
                case ComparisonType.GreaterOrEqual:
                    op = metric.Value > alertRule.ThresholdValue ? "greater than" : "equal to";
                    break;
                case ComparisonType.Equal:
                    op = "equal to";
                    break;
                case ComparisonType.LessThan:
                    op = "less than";
                    break;
                case ComparisonType.LessOrEqual:
                    op = metric.Value < alertRule.ThresholdValue ? "less than" : "equal to";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var metricName = string.IsNullOrWhiteSpace(metric.Instrument)
                ? metric.Name
                : $"{metric.Name} for {metric.Instrument}";

            return isStarted
                ? $"Metric {metricName} has value {metric.Value} that is {op} threshold value {alertRule.ThresholdValue}"
                : $"Alerting for metric {metricName} is off. Current value is {metric.Value} (threshold = {alertRule.ThresholdValue})";
        }
    }
}
