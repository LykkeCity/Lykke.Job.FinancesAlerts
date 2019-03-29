using System.Threading.Tasks;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.HttpClientGenerator.Infrastructure;
using Lykke.Job.FinancesAlerts.Client;
using Lykke.Job.FinancesAlerts.Client.Models;
using Xunit;

namespace Lykke.Job.FinancesAlerts.Tests
{
    public class ClientTests
    {
        private readonly IFinancesAlertsClient _client;

        public ClientTests()
        {
            var clientBuilder = HttpClientGenerator.HttpClientGenerator.BuildForUrl("http://localhost:5000/")
                .WithAdditionalCallsWrapper(new ExceptionHandlerCallsWrapper());

            _client = new FinancesAlertsClient(clientBuilder.Create());
        }

        [Fact(Skip = "Local run test")]
        public async Task AddRuleValidationChecks()
        {
            await Assert.ThrowsAsync<ClientApiException>(async () =>
                await _client.AlertsApi.CreateAlertRuleAsync(new CreateAlertRuleRequest()));

        }
    }
}
