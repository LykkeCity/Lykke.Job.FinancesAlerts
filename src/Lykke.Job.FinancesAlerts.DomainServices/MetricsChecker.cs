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
        private readonly IMetricCalculatorRegistry _metricCalculatorRegistry;

        public MetricsChecker(
            ILogFactory logFactory,
            IAlertRuleRepository alertRuleRepository,
            IMetricCalculatorRegistry metricCalculatorRegistry
        )
        {
            _log = logFactory.CreateLog(this);
            _alertRuleRepository = alertRuleRepository;
            _metricCalculatorRegistry = metricCalculatorRegistry;
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
                var rulesFetchingTask = _alertRuleRepository.GetMetricRulesAsync(metricInfo.Name);

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
                                    InitAlertEvent(metricInfo, metric);
                                break;
                            case ComparisonType.GreaterOrEqual:
                                if (metric.Value >= alertRule.ThresholdValue)
                                    InitAlertEvent(metricInfo, metric);
                                break;
                            case ComparisonType.Equal:
                                if (metric.Value == alertRule.ThresholdValue)
                                    InitAlertEvent(metricInfo, metric);
                                break;
                            case ComparisonType.LessThan:
                                if (metric.Value < alertRule.ThresholdValue)
                                    InitAlertEvent(metricInfo, metric);
                                break;
                            case ComparisonType.LessOrEqual:
                                if (metric.Value <= alertRule.ThresholdValue)
                                    InitAlertEvent(metricInfo, metric);
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

        private void InitAlertEvent(MetricInfo merInfo, Metric metric)
        {
            //TODO
        }
    }
}
