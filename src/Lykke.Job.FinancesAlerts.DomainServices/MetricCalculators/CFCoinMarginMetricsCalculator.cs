using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.FinancesAlerts.Client.Models;
using Lykke.Job.FinancesAlerts.Domain;
using Lykke.Job.FinancesAlerts.Domain.Services;

namespace Lykke.Job.FinancesAlerts.DomainServices.MetricCalculators
{
    public class CfCoinMarginMetricsCalculator : IMetricCalculator
    {
        private const string DataSourceView = "[lyciprogramm].[vCfCoinsGrossMargin]";
        private const string AssetColumnName = "AssetId";
        private const string MetricColumnName = "GrossMargin";

        private readonly ILog _log;
        private readonly ISqlAdapter _sqlAdapter;

        public MetricInfo MetricInfo { get; }

        public CfCoinMarginMetricsCalculator(ILogFactory logFactory, ISqlAdapter sqlAdapter)
        {
            _log = logFactory.CreateLog(this);
            _sqlAdapter = sqlAdapter;

            MetricInfo = new MetricInfo
            {
                Name = "CfCoinMarginDiffPercent",
                Description = "Calculates coin gross margin diff in percents for CryptoFacilities",
                Accuracy = 0,
            };
        }

        public void Dispose()
        {
            StopAsync().GetAwaiter().GetResult();
        }

        public Task<List<Metric>> CalculateMetricsAsync()
        {
            try
            {
                var dataset = _sqlAdapter.GetDataFromTableOrView(DataSourceView);
                var table = dataset.Tables[0];
                var result = new List<Metric>();
                var assetColumnInd = table.Columns.IndexOf(AssetColumnName);
                var metricColumnInd = table.Columns.IndexOf(MetricColumnName);
                foreach (DataRow row in table.Rows)
                {
                    var instrument = row[assetColumnInd].ToString();
                    var metricValueStr = row[metricColumnInd].ToString();
                    var metricValue = decimal.Parse(metricValueStr);
                    result.Add(new Metric
                    {
                        Name = MetricInfo.Name,
                        Value = (metricValue * 100).TruncateDecimalPlaces(MetricInfo.Accuracy + 1),
                        Instrument = instrument,
                    });
                }
                return Task.FromResult(result);
            }
            catch (Exception e)
            {
                _log.Error(e);
                return Task.FromResult(new List<Metric>());
            }
        }

        public Task StartAsync()
        {
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            return Task.CompletedTask;
        }
    }
}
