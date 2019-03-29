using JetBrains.Annotations;

namespace Lykke.Job.FinancesAlerts.Client.Models
{
    [PublicAPI]
    public class DeleteAlertSibscriptionRequest
    {
        public string Id { get; set; }
        public string AlertRuleId { get; set; }
        public string ChangedBy { get; set; }
    }
}
