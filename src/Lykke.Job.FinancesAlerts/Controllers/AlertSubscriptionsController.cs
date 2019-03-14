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
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Lykke.Job.FinancesAlerts.Controllers
{
    [Route("api/subscriptions")]
    public class AlertSubscriptionsController : Controller, IFinancesAlertSubscriptionsApi
    {
        private readonly ILog _log;
        private readonly IAlertSubscriptionRepository _alertSubscriptionRepository;

        public AlertSubscriptionsController(ILogFactory logFactory, IAlertSubscriptionRepository alertSubscriptionRepository)
        {
            _log = logFactory.CreateLog(this);
            _alertSubscriptionRepository = alertSubscriptionRepository;
        }

        [HttpGet("")]
        [SwaggerOperation("GetAlertSubscriptionsData")]
        [ProducesResponseType(typeof(List<AlertSubscription>), (int)HttpStatusCode.OK)]
        public async Task<List<AlertSubscription>> GetAlertSubscriptionsDataAsync(string alertRuleId)
        {
            var subscriptions = await _alertSubscriptionRepository.GetByAlertRuleAsync(alertRuleId);
            return subscriptions.Select(AlertSubscription.Copy).ToList();
        }

        [HttpGet("{alertRuleId}/{alertSubscriptionId}")]
        [SwaggerOperation("GetAlertSubscriptionById")]
        [ProducesResponseType(typeof(AlertSubscription), (int)HttpStatusCode.OK)]
        public async Task<AlertSubscription> GetAlertSubscriptionByIdAsync(string alertRuleId, string alertSubscriptionId)
        {
            var alertSubscription = await _alertSubscriptionRepository.GetAsync(alertRuleId, alertSubscriptionId);
            return AlertSubscription.Copy(alertSubscription);
        }

        [HttpPost("")]
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

        [HttpPut("")]
        [SwaggerOperation("UpdateAlertSubscription")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task UpdateAlertSubscriptionAsync([FromBody] UpdateAlertSibscriptionRequest request)
        {
            var existing = await _alertSubscriptionRepository.GetAsync(request.AlertRuleId, request.Id);
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
                });
        }

        [HttpDelete("")]
        [SwaggerOperation("DeleteAlertSubscription")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public Task DeleteAlertSubscriptionAsync(DeleteAlertSibscriptionRequest request)
        {
            _log.Info(nameof(DeleteAlertSubscriptionAsync), request.ChangedBy, request);

            return _alertSubscriptionRepository.DeleteAsync(request.AlertRuleId, request.Id);
        }
    }
}
