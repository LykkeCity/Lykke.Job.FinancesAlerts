using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Job.FinancesAlerts.Client.Models;

namespace Lykke.Job.FinancesAlerts.Domain.Services
{
    public interface IMetricCalculatorRegistry
    {
        List<IMetricCalculator> GetAllMetricCalculators();

        List<MetricInfo> GetAvailableMetrics();

        Task StartAsync();

        Task StopAsync();
    }
}
