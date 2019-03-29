using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Job.FinancesAlerts.Client.Models;

namespace Lykke.Job.FinancesAlerts.Domain.Services
{
    public interface IMetricCalculator
    {
        MetricInfo MetricInfo { get; }

        Task<List<Metric>> CalculateMetricsAsync();

        Task StartAsync();

        Task StopAsync();
    }
}
