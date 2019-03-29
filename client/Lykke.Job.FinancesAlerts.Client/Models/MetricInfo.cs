using JetBrains.Annotations;

namespace Lykke.Job.FinancesAlerts.Client.Models
{
    [PublicAPI]
    public class MetricInfo
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Accuracy { get; set; }
    }
}
