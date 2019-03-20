using System.Data;

namespace Lykke.Job.FinancesAlerts.Domain.Services
{
    public interface ISqlAdapter
    {
        DataSet GetDataFromTableOrView(string table);
    }
}
