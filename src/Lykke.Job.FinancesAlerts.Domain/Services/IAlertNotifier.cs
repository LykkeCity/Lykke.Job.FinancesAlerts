using System.Threading.Tasks;
using Lykke.Job.FinancesAlerts.Client.Models;

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
