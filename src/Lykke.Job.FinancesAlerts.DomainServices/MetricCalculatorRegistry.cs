using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Job.FinancesAlerts.Client.Models;
using Lykke.Job.FinancesAlerts.Domain.Services;

namespace Lykke.Job.FinancesAlerts.DomainServices
{
    public class MetricCalculatorRegistry : IMetricCalculatorRegistry
    {
        private readonly  Dictionary<string, IMetricCalculator> _calculators = new Dictionary<string, IMetricCalculator>();

        public MetricCalculatorRegistry(IEnumerable<IMetricCalculator> metricCalculators)
        {
            foreach (var metricCalculator in metricCalculators)
            {
                _calculators.Add(metricCalculator.MetricInfo.Name, metricCalculator);
            }
        }

        public async Task StartAsync()
        {
            foreach (var calculator in _calculators.Values)
            {
                await calculator.StartAsync();
            }
        }

        public async Task StopAsync()
        {
            foreach (var calculator in _calculators.Values)
            {
                await calculator.StopAsync();
            }
        }

        public List<IMetricCalculator> GetAllMetricCalculators()
        {
            return _calculators.Values.ToList();
        }

        public List<MetricInfo> GetAvailableMetrics()
        {
            return _calculators.Values.Select(c => c.MetricInfo).ToList();
        }
    }
}
