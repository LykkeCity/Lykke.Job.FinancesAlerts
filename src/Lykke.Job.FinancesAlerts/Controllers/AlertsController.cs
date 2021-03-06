﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Common.Log;
using Lykke.Job.FinancesAlerts.Client;
using Lykke.Job.FinancesAlerts.Client.Models;
using Lykke.Job.FinancesAlerts.Domain.Repositories;
using Lykke.Job.FinancesAlerts.Domain.Services;
using Lykke.Job.FinancesAlerts.Extensions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Lykke.Job.FinancesAlerts.Controllers
{
    [Route("api/alerts")]
    public class AlertsController : Controller, IFinancesAlertsApi
    {
        private readonly ILog _log;
        private readonly IAlertRuleRepository _alertRuleRepository;
        private readonly IAlertSubscriptionRepository _alertSubscriptionRepository;
        private readonly IMetricCalculatorRegistry _metricCalculatorRegistry;

        public AlertsController(
            ILogFactory logFactory,
            IAlertRuleRepository alertRuleRepository,
            IAlertSubscriptionRepository alertSubscriptionRepository,
            IMetricCalculatorRegistry metricCalculatorRegistry)
        {
            _log = logFactory.CreateLog(this);
            _alertRuleRepository = alertRuleRepository;
            _alertSubscriptionRepository = alertSubscriptionRepository;
            _metricCalculatorRegistry = metricCalculatorRegistry;
        }

        [HttpGet("")]
        [SwaggerOperation("GetAlertRulesData")]
        [ProducesResponseType(typeof(AlertRulesData), (int)HttpStatusCode.OK)]
        public async Task<AlertRulesData> GetAlertRulesDataAsync()
        {
            var metrics = _metricCalculatorRegistry.GetAvailableMetrics();
            var alertRules = new List<AlertRule>();
            foreach (var metric in metrics)
            {
                var metricAlertRules = await _alertRuleRepository.GetByMetricAsync(metric.Name);
                foreach (var metricAlertRule in metricAlertRules)
                {
                    var subscriptions = await _alertSubscriptionRepository.GetByAlertRuleAsync(metricAlertRule.Id);
                    alertRules.Add(metricAlertRule.ToClient(subscriptions.Count()));
                }
            }

            return new AlertRulesData
            {
                AlertRules = alertRules,
                Metrics = metrics.Select(i => i.ToClient()).ToList(),
            };
        }

        [HttpGet("metrics")]
        [SwaggerOperation("GetAlertRulesMetrics")]
        [ProducesResponseType(typeof(List<MetricInfo>), (int)HttpStatusCode.OK)]
        public Task<List<MetricInfo>> GetAlertRulesMetricsAsync()
        {
            var metrics = _metricCalculatorRegistry.GetAvailableMetrics()
                .Select(i => i.ToClient())
                .ToList();
            return Task.FromResult(metrics);
        }

        [HttpGet("{alertRuleId}/metrics/{metricName}")]
        [SwaggerOperation("GetAlertRuleById")]
        [ProducesResponseType(typeof(AlertRule), (int) HttpStatusCode.OK)]
        public async Task<AlertRule> GetAlertRuleByIdAsync(string metricName, string alertRuleId)
        {
            var alertRule = await _alertRuleRepository.GetAsync(metricName, alertRuleId);
            var subscriptionsCount = 0;
            if (alertRule != null)
            {
                var subscriptions = await _alertSubscriptionRepository.GetByAlertRuleAsync(alertRule.Id);
                subscriptionsCount = subscriptions.Count();
            }
            return alertRule.ToClient(subscriptionsCount);
        }

        [HttpPost("")]
        [SwaggerOperation("CreateAlertRule")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public Task<string> CreateAlertRuleAsync([FromBody] CreateAlertRuleRequest request)
        {
            _log.Info(nameof(CreateAlertRuleAsync), request.ChangedBy, request);

            return _alertRuleRepository.AddAsync(
                request.MetricName,
                request.ComparisonType.ToDomain(),
                request.ThresholdValue,
                request.ChangedBy);
        }

        [HttpPut("")]
        [SwaggerOperation("UpdateAlertRule")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task UpdateAlertRuleAsync([FromBody] UpdateAlertRuleRequest request)
        {
            var existing = await _alertRuleRepository.GetAsync(request.MetricName, request.Id);
            if (existing == null)
                throw new ValidationApiException("Alert rule not found");

            _log.Info(nameof(UpdateAlertRuleAsync), request.ChangedBy, request);

            await _alertRuleRepository.UpdateAsync(
                request.Id,
                request.MetricName,
                request.ComparisonType.ToDomain(),
                request.ThresholdValue,
                request.ChangedBy);
        }

        [HttpDelete("")]
        [SwaggerOperation("DeleteAlertRule")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public Task DeleteAlertRuleAsync(DeleteAlertRuleRequest request)
        {
            _log.Info(nameof(DeleteAlertRuleAsync), request.ChangedBy, request);

            return _alertRuleRepository.DeleteAsync(request.MetricName, request.Id);
        }
    }
}
