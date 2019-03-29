using System;
using Lykke.Job.FinancesAlerts.Domain;

namespace Lykke.Job.FinancesAlerts.Extensions
{
    internal static class SubscriptionTypeMapExtensions
    {
        internal static Client.Models.AlertSubscriptionType ToClient(this AlertSubscriptionType subscriptionType)
        {
            switch (subscriptionType)
            {
                case AlertSubscriptionType.Email:
                    return Client.Models.AlertSubscriptionType.Email;
                case AlertSubscriptionType.Sms:
                    return Client.Models.AlertSubscriptionType.Sms;
                default:
                    throw new ArgumentOutOfRangeException(nameof(subscriptionType), subscriptionType, null);
            }
        }

        internal static AlertSubscriptionType ToDomain(this Client.Models.AlertSubscriptionType subscriptionType)
        {
            switch (subscriptionType)
            {
                case Client.Models.AlertSubscriptionType.Email:
                    return AlertSubscriptionType.Email;
                case Client.Models.AlertSubscriptionType.Sms:
                    return AlertSubscriptionType.Sms;
                default:
                    throw new ArgumentOutOfRangeException(nameof(subscriptionType), subscriptionType, null);
            }
        }
    }
}
