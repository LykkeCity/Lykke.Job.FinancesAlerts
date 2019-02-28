using Lykke.Job.FinancesAlerts.Settings.JobSettings;
using Lykke.Sdk.Settings;
using Lykke.Service.SmsSender.Client;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.FinancesAlerts.Settings
{
    public class AppSettings : BaseAppSettings
    {
        public FinancesAlertsJobSettings FinancesAlertsJob { get; set; }
        public SmsSenderServiceClientSettings SmsSenderServiceClient { get; set; }
        public EmailPartnerRouterServiceClientSettings EmailPartnerRouterServiceClient { get; set; }
    }

    public class EmailPartnerRouterServiceClientSettings
    {
        [HttpCheck("api/isalive")]
        public string ServiceUrl { get; set; }
    }
}
