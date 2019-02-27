using Lykke.CryptoFacilities;

namespace Lykke.Job.FinancesAlerts.Settings.JobSettings
{
    public class FinancesAlertsJobSettings
    {
        public DbSettings Db { get; set; }

        public CryptoFacilitiesApiSettings CryptoFacilities { get; set; }
    }
}
