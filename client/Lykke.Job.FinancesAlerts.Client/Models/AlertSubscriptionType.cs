using System;

namespace Lykke.Job.FinancesAlerts.Client.Models
{
    [Flags]
    public enum AlertSubscriptionType : byte
    {
        Email = 0,
        Sms = 1,
    }
}
