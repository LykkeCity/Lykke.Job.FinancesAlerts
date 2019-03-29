using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.Job.FinancesAlerts.Client.Models
{
    [PublicAPI]
    public class AlertRulesData
    {
        public List<AlertRule> AlertRules { get; set; }

        public List<MetricInfo> Metrics { get; set; }
    }
}
