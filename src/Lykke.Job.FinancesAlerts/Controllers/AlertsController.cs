using System;
using System.Collections.Generic;
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

        [HttpGet]
        [Route("")]
        [SwaggerOperation("GetAlertRulesData")]
        [ProducesResponseType(typeof(AlertRulesData), (int)HttpStatusCode.OK)]
        public async Task<AlertRulesData> GetAlertRulesDataAsync()
        {
            var metrics = _metricCalculatorRegistry.GetAvailableMetrics();
            var alertRules = new List<AlertRule>();
            foreach (var metric in metrics)
            {
                var metricAlertRules = await _alertRuleRepository.GetByMetricAsync(metric.Name).ConfigureAwait(false);
                foreach (var metricAlertRule in metricAlertRules)
                {
                    var subscriptions = await _alertSubscriptionRepository.GetByAlertRuleAsync(metricAlertRule.Id).ConfigureAwait(false);
                    if (!subscriptions.Any())
                        continue;

                    var alertRule = AlertRule.Copy(metricAlertRule);
                    alertRule.SubscriptionsCount = subscriptions.Count();
                    alertRules.Add(alertRule);
                }
            }

            return new AlertRulesData
            {
                AlertRules = alertRules,
                Metrics = metrics,
                ComparisonTypes = Enum.GetValues(typeof(ComparisonType)).Cast<ComparisonType>().ToList(),
            };
        }

        [HttpPost]
        [Route("addrule")]
        [SwaggerOperation("CreateAlertRule")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public Task<string> CreateAlertRuleAsync([FromBody] CreateAlertRuleRequest request)
        {
            _log.Info(nameof(CreateAlertRuleAsync), request.ChangedBy, request);

            return _alertRuleRepository.AddAsync(
                new AlertRule
                {
                    MetricName = request.MetricName,
                    ComparisonType = request.ComparisonType,
                    ThresholdValue = request.ThresholdValue,
                    ChangedBy = request.ChangedBy,
                });
        }

        [HttpPut]
        [Route("updaterule")]
        [SwaggerOperation("UpdateAlertRule")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task UpdateAlertRuleAsync([FromBody] UpdateAlertRuleRequest request)
        {
            var existing = await _alertRuleRepository.GetAsync(request.MetricName, request.Id).ConfigureAwait(false);
            if (existing == null)
                throw new ValidationApiException("Alert rule not found");

            _log.Info(nameof(UpdateAlertRuleAsync), request.ChangedBy, request);

            await _alertRuleRepository.UpdateAsync(
                new AlertRule
                {
                    Id = request.Id,
                    MetricName = request.MetricName,
                    ComparisonType = request.ComparisonType,
                    ThresholdValue = request.ThresholdValue,
                    ChangedBy = request.ChangedBy,
                }).ConfigureAwait(false);
        }

        [HttpDelete]
        [Route("deleterule")]
        [SwaggerOperation("DeleteAlertRule")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public Task DeleteAlertRuleAsync([FromBody] DeleteAlertRuleRequest request)
        {
            _log.Info(nameof(DeleteAlertRuleAsync), request.ChangedBy, request);

            return _alertRuleRepository.DeleteAsync(request.MetricName, request.Id);
        }

        [HttpGet]
        [Route("subscriptions")]
        [SwaggerOperation("GetAlertSubscriptionsData")]
        [ProducesResponseType(typeof(AlertRuleSubscriptionsData), (int)HttpStatusCode.OK)]
        public async Task<AlertRuleSubscriptionsData> GetAlertSubscriptionsDataAsync(string alertRuleId)
        {
            var subscriptions = await _alertSubscriptionRepository.GetByAlertRuleAsync(alertRuleId).ConfigureAwait(false);
            return new AlertRuleSubscriptionsData
            {
                AlertRuleId = alertRuleId,
                Subscriptions = subscriptions.Select(AlertSubscription.Copy).ToList(),
                SubscriptionTypes = Enum.GetValues(typeof(AlertSubscriptionType)).Cast<AlertSubscriptionType>().ToList(),
            };
        }

        [HttpPost]
        [Route("addsubscription")]
        [SwaggerOperation("CreateAlertSubscription")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public Task<string> CreateAlertSibscriptionAsync([FromBody] CreateAlertSibscriptionRequest request)
        {
            _log.Info(nameof(CreateAlertSibscriptionAsync), request.ChangedBy, request);

            return _alertSubscriptionRepository.AddAsync(
                new AlertSubscription
                {
                    AlertRuleId = request.AlertRuleId,
                    Address = request.Address,
                    Type = request.Type,
                    AlertFrequency = request.AlertFrequency,
                    ChangedBy = request.ChangedBy,
                });
        }

        [HttpPut]
        [Route("updatesubscription")]
        [SwaggerOperation("UpdateAlertSubscription")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task UpdateAlertSubscriptionAsync([FromBody] UpdateAlertSibscriptionRequest request)
        {
            var existing = await _alertSubscriptionRepository.GetAsync(request.AlertRuleId, request.Id).ConfigureAwait(false);
            if (existing == null)
                throw new ValidationApiException("Alert subscription not found");

            _log.Info(nameof(UpdateAlertSubscriptionAsync), request.ChangedBy, request);

            await _alertSubscriptionRepository.UpdateAsync(
                new AlertSubscription
                {
                    Id = request.Id,
                    AlertRuleId = request.AlertRuleId,
                    Address = request.Address,
                    Type = request.Type,
                    AlertFrequency = request.AlertFrequency,
                    ChangedBy = request.ChangedBy,
                }).ConfigureAwait(false);
        }

        [HttpDelete]
        [Route("deletesubscription")]
        [SwaggerOperation("DeleteAlertSubscription")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public Task DeleteAlertSubscriptionAsync([FromBody] DeleteAlertSibscriptionRequest request)
        {
            _log.Info(nameof(DeleteAlertSubscriptionAsync), request.ChangedBy, request);

            return _alertSubscriptionRepository.DeleteAsync(request.AlertRuleId, request.Id);
        }
    }
}
