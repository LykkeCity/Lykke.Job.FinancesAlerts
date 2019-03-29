using System.Threading.Tasks;

namespace Lykke.Job.FinancesAlerts.Domain.Repositories
{
    public interface ISmsMockRepository
    {
        Task InsertAsync(string phoneNumber, string msg);
    }
}
