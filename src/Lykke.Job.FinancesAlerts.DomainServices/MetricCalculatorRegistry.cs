using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                await calculator.StartAsync().ConfigureAwait(false);
            }
        }

        public List<IMetricCalculator> GetAllMetricCalculators()
        {
            return _calculators.Values.ToList();
        }
    }
}
