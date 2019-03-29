using System.Threading.Tasks;

namespace Lykke.Job.FinancesAlerts.Domain.Services
{
    public interface IAlertNotifier
    {
        Task NotifyAsync(
            AlertSubscriptionType subscriptionType,
            string address,
            string topic,
            string message);
    }
}
