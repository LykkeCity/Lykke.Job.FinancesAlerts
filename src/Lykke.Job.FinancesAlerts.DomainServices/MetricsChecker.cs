using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.FinancesAlerts.Domain;
using Lykke.Job.FinancesAlerts.Domain.Repositories;
using Lykke.Job.FinancesAlerts.Domain.Services;

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

        public MetricsChecker(
            ILogFactory logFactory,
            IAlertRuleRepository alertRuleRepository,
            IAlertSubscriptionRepository alertSubscriptionRepository,
            IMetricCalculatorRegistry metricCalculatorRegistry,
            IAlertNotifier alertNotifier)
        {
            _log = logFactory.CreateLog(this);
            _alertRuleRepository = alertRuleRepository;
            _alertSubscriptionRepository = alertSubscriptionRepository;
            _metricCalculatorRegistry = metricCalculatorRegistry;
            _alertNotifier = alertNotifier;
        }

        public async Task CheckAllMetricsAsync()
        {
            var metricCalculators = _metricCalculatorRegistry.GetAllMetricCalculators();

            await Task.WhenAll(metricCalculators.Select(CheckMetricAsync)).ConfigureAwait(false);
        }

        private async Task CheckMetricAsync(IMetricCalculator metricCalculator)
        {
            try
            {
                var metricInfo = metricCalculator.MetricInfo;

                var calculationTask = metricCalculator.CalculateMetricsAsync();
                var rulesFetchingTask = _alertRuleRepository.GetByMetricAsync(metricInfo.Name);

                await Task.WhenAll(calculationTask, rulesFetchingTask).ConfigureAwait(false);

                var alertRules = rulesFetchingTask.Result;
                var metrics = calculationTask.Result;

                foreach (var metric in metrics)
                {
                    foreach (var alertRule in alertRules)
                    {
                        bool needToFireAlert = false;
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
                                .ConfigureAwait(false);
                        }
                        else if (_activeAlerts.Contains(alertRule.Id))
                        {
                            await ProcessAlertEventAsync(
                                    metric,
                                    alertRule,
                                    false)
                                .ConfigureAwait(false);
                            _activeAlerts.Remove(alertRule.Id);
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
            var subscriptions = await _alertSubscriptionRepository.GetByAlertRuleAsync(alertRule.Id).ConfigureAwait(false);
            if (!subscriptions.Any())
                return;

            _activeAlerts.Add(alertRule.Id);

            var message = GenerateAlerMessage(
                metric,
                alertRule,
                isStarted);
            foreach (var subscription in subscriptions)
            {
                var alertSubKey = $"{metric.Name}_{subscription.Type}_{isStarted}";
                var now = DateTime.UtcNow;
                if (_lastAlertTimeDict.TryGetValue(alertSubKey, out var lastAlertTime))
                {
                    if (now - lastAlertTime < subscription.AlertFrequency)
                        continue;
                }

                _lastAlertTimeDict[alertSubKey] = now;

                await _alertNotifier.NotifyAsync(
                    subscription.Type,
                    subscription.SubscriptionData,
                    $"{metric.Name} alert",
                    message)
                    .ConfigureAwait(false);
            }
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

            var metricName = string.IsNullOrWhiteSpace(metric.Info)
                ? metric.Name
                : $"{metric.Name} for {metric.Info}";

            return isStarted
                ? $"Metric {metricName} has value {metric.Value} that is {op} threshold value {alertRule.ThresholdValue}"
                : $"Alerting for metric {metricName} is off. Current value is {metric.Value} (threshold = {alertRule.ThresholdValue})";
        }
    }
}
