using System.Data;
using System.Data.SqlClient;
using Lykke.Job.FinancesAlerts.Domain.Services;

namespace Lykke.Job.FinancesAlerts.DomainServices
{
    public class SqlAdapter : ISqlAdapter
    {
        private readonly string _sqlConnString;

        public SqlAdapter(string sqlConnString)
        {
            _sqlConnString = sqlConnString;
        }

        public DataSet GetDataFromTableOrView(string table)
        {
            using (var connection = new SqlConnection(_sqlConnString))
            using (var command = connection.CreateCommand())
            {
                command.CommandTimeout = 600;
                connection.Open();
                command.CommandText = $"SELECT * FROM {table}";
                command.CommandType = CommandType.StoredProcedure;
                var dataAdapter = new SqlDataAdapter(command);
                var result = new DataSet();
                dataAdapter.Fill(result);
                return result;
            }
        }
    }
}
