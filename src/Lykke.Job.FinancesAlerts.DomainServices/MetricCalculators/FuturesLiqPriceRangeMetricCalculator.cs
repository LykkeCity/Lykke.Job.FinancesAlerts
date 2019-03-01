using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Common.Log;
using Lykke.CryptoFacilities;
using Lykke.CryptoFacilities.WebSockets;
using Lykke.CryptoFacilities.WebSockets.FeedMessages;
using Lykke.Job.FinancesAlerts.Client.Models;
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
                Accuracy = 0,
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

        public Task<List<Metric>> CalculateMetricsAsync()
        {
            var metrics = _instumentsMetricDictionary.Select(p =>
                new Metric
                {
                    Name = MetricInfo.Name,
                    Value = p.Value,
                    Info = p.Key,
                })
                .ToList();

            return Task.FromResult(metrics);
        }

        private Task HandlerMessageAsync(OpenPositionsMessage m)
        {
            foreach (var positionMessage in m.Positions)
            {
                var value = (positionMessage.MarkPrice - positionMessage.LiquidationThreshold) / positionMessage.MarkPrice * 100;
                value = value.TruncateDecimalPlaces(MetricInfo.Accuracy + 1);
                _instumentsMetricDictionary[positionMessage.Instrument] = value;
            }

            return Task.CompletedTask;
        }
    }
}
