using Lykke.Job.FinancesAlerts.Domain;

namespace Lykke.Job.FinancesAlerts.Extensions
{
    internal static class MetricInfoMapExtensions
    {
        internal static Client.Models.MetricInfo ToClient(this MetricInfo metricInfo)
        {
            return new Client.Models.MetricInfo
            {
                Name = metricInfo.Name,
                Description = metricInfo.Description,
                Accuracy = metricInfo.Accuracy,
            };
        }
    }
}
