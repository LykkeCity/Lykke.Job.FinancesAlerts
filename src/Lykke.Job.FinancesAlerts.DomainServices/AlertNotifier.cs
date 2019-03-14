using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.FinancesAlerts.Client.Models;
using Lykke.Job.FinancesAlerts.Domain;
using Lykke.Job.FinancesAlerts.Domain.Services;
using Lykke.Service.EmailPartnerRouter;
using Lykke.Service.EmailSender;
using Lykke.Service.SmsSender.Client;

namespace Lykke.Job.FinancesAlerts.DomainServices
{
    public class AlertNotifier : IAlertNotifier
    {
        private readonly ILog _log;
        private readonly ISmsSenderClient _smsSenderClient;
        private readonly IEmailPartnerRouter _emailPartnerRouter;

        public AlertNotifier(
            ILogFactory logFactory,
            ISmsSenderClient smsSenderClient,
            IEmailPartnerRouter emailPartnerRouter)
        {
            _log = logFactory.CreateLog(this);
            _smsSenderClient = smsSenderClient;
            _emailPartnerRouter = emailPartnerRouter;
        }

        public async Task NotifyAsync(
            AlertSubscriptionType subscriptionType,
            string address,
            string topic,
            string message)
        {
            try
            {
                switch (subscriptionType)
                {
                    case AlertSubscriptionType.Email:
                        await NotifyViaEmailAsync(address, topic, message);
                        break;
                    case AlertSubscriptionType.Sms:
                        await NotifyViaSmsAsync(address, topic, message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(subscriptionType), subscriptionType, null);
                }
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
        }

        private async Task NotifyViaSmsAsync(
            string address,
            string topic,
            string message)
        {
            _log.Info(topic, message, address.SanitizePhone());

            await _smsSenderClient.SendSmsAsync(address, $"{topic} : {message}");
        }

        private async Task NotifyViaEmailAsync(
            string address,
            string topic,
            string message)
        {
            _log.Info(topic, message, address.SanitizeEmail());

            await _emailPartnerRouter.SendAsync(
                null,
                new EmailMessage
                {
                    Subject = topic,
                    TextBody = message,
                },
                new EmailAddressee
                {
                    EmailAddress = address
                })
                ;
        }
    }
}
