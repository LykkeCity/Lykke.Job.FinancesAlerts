using System;
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
                        switch (alertRule.ComparisonType)
                        {
                            case ComparisonType.GreaterThan:
                                if (metric.Value > alertRule.ThresholdValue)
                                    await InitAlertEventAsync(metric, alertRule).ConfigureAwait(false);
                                break;
                            case ComparisonType.GreaterOrEqual:
                                if (metric.Value >= alertRule.ThresholdValue)
                                    await InitAlertEventAsync(metric, alertRule).ConfigureAwait(false);
                                break;
                            case ComparisonType.Equal:
                                if (metric.Value == alertRule.ThresholdValue)
                                    await InitAlertEventAsync(metric, alertRule).ConfigureAwait(false);
                                break;
                            case ComparisonType.LessThan:
                                if (metric.Value < alertRule.ThresholdValue)
                                    await InitAlertEventAsync(metric, alertRule).ConfigureAwait(false);
                                break;
                            case ComparisonType.LessOrEqual:
                                if (metric.Value <= alertRule.ThresholdValue)
                                    await InitAlertEventAsync(metric, alertRule).ConfigureAwait(false);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
        }

        private async Task InitAlertEventAsync(Metric metric, IAlertRule alertRule)
        {
            var subscriptions = await _alertSubscriptionRepository.GetByAlertRuleAsync(alertRule.Id).ConfigureAwait(false);
            if (!subscriptions.Any())
                return;

            var message = GenerateAlerMessage(metric, alertRule);
            foreach (var subscription in subscriptions)
            {
                await _alertNotifier.NotifyAsync(
                    subscription.Type,
                    subscription.SubscriptionData,
                    $"{metric.Name} alert",
                    message)
                    .ConfigureAwait(false);
            }
        }

        private string GenerateAlerMessage(Metric metric, IAlertRule alertRule)
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

            return $"Metric {metric.Name} has value {metric.Value} that is {op} threshold value {alertRule.ThresholdValue}";
        }
    }
}
