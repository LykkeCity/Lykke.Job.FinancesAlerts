namespace Lykke.Job.FinancesAlerts.Client.Models
{
    public class DeleteAlertSibscriptionRequest
    {
        public string Id { get; set; }
        public string AlertRuleId { get; set; }
        public string ChangedBy { get; set; }
    }
}
