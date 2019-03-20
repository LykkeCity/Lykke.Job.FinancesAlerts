using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.FinancesAlerts.Settings.JobSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }

        [AzureTableCheck]
        public string DataConnString { get; set; }

        [SqlCheck]
        public string SqlConnString { get; set; }
    }
}
