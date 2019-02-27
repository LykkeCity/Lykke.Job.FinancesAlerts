using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.FinancesAlerts.Domain.Services
{
    public interface IMetricCalculator : IDisposable
    {
        MetricInfo MetricInfo { get; }

        Task<IEnumerable<Metric>> CalculateMetricsAsync();

        Task StartAsync();

        Task StopAsync();
    }
}
