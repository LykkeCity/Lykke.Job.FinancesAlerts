using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Common.Log;
using Lykke.CryptoFacilities.WebSockets;
using Lykke.CryptoFacilities.WebSockets.FeedMessages;
using Lykke.Job.FinancesAlerts.Client.Models;
using Lykke.Job.FinancesAlerts.Domain;
using Lykke.Job.FinancesAlerts.Domain.Services;

namespace Lykke.Job.FinancesAlerts.DomainServices.MetricCalculators
{
    public class CfFuturesLiqPriceRangeMetricCalculator : IMetricCalculator, IDisposable
    {
        private readonly ILogFactory _logFactory;
        private readonly string _apiPath;
        private readonly string _apiPrivateKey;
        private readonly string _apiPublicKey;
        private readonly string _openPositionsFeed;
        private readonly Dictionary<string, decimal> _instumentsMetricDictionary = new Dictionary<string, decimal>();

        private PrivateCryptoFacilitiesConnection<OpenPositionsMessage, OpenPositionsMessage> _privateCfWsClient;

        public MetricInfo MetricInfo { get; }

        public CfFuturesLiqPriceRangeMetricCalculator(
            ILogFactory logFactory,
            string apiPath,
            string apiPrivateKey,
            string apiPublicKey,
            string openPositionsFeed)
        {
            _logFactory = logFactory;
            _apiPath = apiPath;
            _apiPrivateKey = apiPrivateKey;
            _apiPublicKey = apiPublicKey;
            _openPositionsFeed = openPositionsFeed;

            MetricInfo = new MetricInfo
            {
                Name = "CfFuturesLiqPriceDiffPercent",
                Description = "Calculates futures contract liquidation price diff angainst its market price in percents for CryptoFacilities",
                Accuracy = 0,
            };
        }

        public async Task StartAsync()
        {
            if (_privateCfWsClient != null)
                throw new InvalidOperationException($"{nameof(CfFuturesLiqPriceRangeMetricCalculator)} is already started");

            _privateCfWsClient = new PrivateCryptoFacilitiesConnection<OpenPositionsMessage, OpenPositionsMessage>(
                _apiPath,
                _openPositionsFeed,
                HandlerMessageAsync,
                HandlerMessageAsync,
                _apiPrivateKey,
                _apiPublicKey,
                TimeSpan.FromSeconds(60),
                _logFactory);

            await _privateCfWsClient.Start();
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
                    Instrument = p.Key,
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
