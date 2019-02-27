using System.Threading.Tasks;

namespace Lykke.Job.FinancesAlerts.Domain.Services
{
    public interface IMetricsChecker
    {
        Task CheckAllMetricsAsync();
    }
}
