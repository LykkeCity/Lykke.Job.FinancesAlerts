using System.Collections.Generic;

namespace Lykke.Job.FinancesAlerts.Client.Models
{
    public class AlertRulesData
    {
        public List<AlertRule> AlertRules { get; set; }

        public List<MetricInfo> Metrics { get; set; }

        public List<ComparisonType> ComparisonTypes { get; set; }
    }
}
