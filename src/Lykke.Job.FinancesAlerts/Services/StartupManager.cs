using System.Threading.Tasks;
using Lykke.Job.FinancesAlerts.Domain.Services;
using Lykke.Job.FinancesAlerts.PeriodicalHandlers;
using Lykke.Sdk;

namespace Lykke.Job.FinancesAlerts.Services
{
    public class StartupManager : IStartupManager
    {
        private readonly IMetricCalculatorRegistry _metricCalculatorRegistry;
        private readonly PeriodicalHandler _mainTimer;

        public StartupManager(
            IMetricCalculatorRegistry metricCalculatorRegistry,
            PeriodicalHandler mainTimer)
        {
            _metricCalculatorRegistry = metricCalculatorRegistry;
            _mainTimer = mainTimer;
        }

        public async Task StartAsync()
        {
            await _metricCalculatorRegistry.StartAsync().ConfigureAwait(false);

            _mainTimer.Start();
        }
    }
}
