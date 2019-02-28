using System.Threading.Tasks;
using Lykke.Job.FinancesAlerts.Domain.Repositories;
using Lykke.Service.SmsSender.Client;

namespace Lykke.Job.FinancesAlerts.DomainServices
{
    public class SmsMockSender : ISmsSenderClient
    {
        private readonly ISmsMockRepository _smsMockRepository;

        public SmsMockSender(ISmsMockRepository smsMockRepository)
        {
            _smsMockRepository = smsMockRepository;
        }

        public async Task SendSmsAsync(string phone, string message)
        {
            await _smsMockRepository.InsertAsync(phone, message);
        }
    }
}
