using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.CryptoFacilities;
using Lykke.CryptoFacilities.WebSockets;
using Lykke.CryptoFacilities.WebSockets.FeedMessages;
using Lykke.Job.FinancesAlerts.Domain;
using Lykke.Job.FinancesAlerts.Domain.Services;

namespace Lykke.Job.FinancesAlerts.DomainServices.MetricCalculators
{
    public class FuturesLiqPriceRangeMetricCalculator : IMetricCalculator
    {
        private readonly ILogFactory _logFactory;
        private readonly CryptoFacilitiesApiSettings _cryptoFacilitiesApiSettings;
        private readonly Dictionary<string, decimal> _instumentsMetricDictionary = new Dictionary<string, decimal>();

        private PrivateCryptoFacilitiesConnection<OpenPositionsMessage, OpenPositionsMessage> _privateCfWsClient;

        public MetricInfo MetricInfo { get; }

        public FuturesLiqPriceRangeMetricCalculator(ILogFactory logFactory, CryptoFacilitiesApiSettings cryptoFacilitiesApiSettings)
        {
            _logFactory = logFactory;
            _cryptoFacilitiesApiSettings = cryptoFacilitiesApiSettings;

            MetricInfo = new MetricInfo
            {
                Name = "FuturesLiqPriceDiffPercent",
                Description = "Calculates futures contract liquidation price diff angainst its market price in percents",
            };
        }

        public async Task StartAsync()
        {
            if (_privateCfWsClient != null)
                throw new InvalidOperationException($"{nameof(FuturesLiqPriceRangeMetricCalculator)} is already started");

            _privateCfWsClient = new PrivateCryptoFacilitiesConnection<OpenPositionsMessage, OpenPositionsMessage>(
                _cryptoFacilitiesApiSettings.ApiPath,
                "open_positions",
                HandlerMessageAsync,
                HandlerMessageAsync,
                _cryptoFacilitiesApiSettings.ApiPrivateKey,
                _cryptoFacilitiesApiSettings.ApiPublicKey,
                TimeSpan.FromSeconds(60),
                _logFactory);

            await _privateCfWsClient.Start().ConfigureAwait(false);
        }

        public async Task StopAsync()
        {
            if (_privateCfWsClient != null)
                await _privateCfWsClient.Stop();

            _privateCfWsClient?.Dispose();
            _privateCfWsClient = null;
        }

        public void Dispose()
        {
            StopAsync().GetAwaiter().GetResult();
        }

        public Task<IEnumerable<Metric>> CalculateMetricsAsync()
        {
            var metrics = _instumentsMetricDictionary.Values.Select(v =>
                new Metric
                {
                    Name = MetricInfo.Name,
                    Value = v,
                });

            return Task.FromResult(metrics);
        }

        private Task HandlerMessageAsync(OpenPositionsMessage m)
        {
            foreach (var positionMessage in m.Positions)
            {
                _instumentsMetricDictionary[positionMessage.Instrument] =
                    (positionMessage.MarkPrice - positionMessage.LiquidationThreshold) / positionMessage.MarkPrice;
            }

            return Task.CompletedTask;
        }
    }
}
