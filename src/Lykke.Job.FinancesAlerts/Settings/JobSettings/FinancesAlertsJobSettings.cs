using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.FinancesAlerts.Settings.JobSettings
{
    public class FinancesAlertsJobSettings
    {
        public DbSettings Db { get; set; }

        public CryptoFacilitiesSettings CryptoFacilities { get; set; }

        public string CoinGrossMarginView { get; set; }

        [Optional]
        public bool UseSmsMocks { get; set; }

        [Optional]
        public string[] DisabledMetrics { get; set; }
    }
}
