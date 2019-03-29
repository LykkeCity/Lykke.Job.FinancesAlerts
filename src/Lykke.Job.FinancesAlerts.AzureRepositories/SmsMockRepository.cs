using System.Threading.Tasks;
using AzureStorage;
using Lykke.Job.FinancesAlerts.Domain.Repositories;

namespace Lykke.Job.FinancesAlerts.AzureRepositories
{
    public class SmsMockRepository : ISmsMockRepository
    {
        private readonly INoSQLTableStorage<SmsMessageMockEntity> _tableStorage;

        public SmsMockRepository(INoSQLTableStorage<SmsMessageMockEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public Task InsertAsync(string phoneNumber, string msg)
        {
            var newEntity = SmsMessageMockEntity.Create(phoneNumber, "SMS mock sender", msg);
            return _tableStorage.InsertAsync(newEntity);
        }
    }
}
