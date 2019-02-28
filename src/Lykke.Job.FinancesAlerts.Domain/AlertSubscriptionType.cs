using System;

namespace Lykke.Job.FinancesAlerts.Domain
{
    [Flags]
    public enum AlertSubscriptionType : byte
    {
        Email = 0,
        Sms = 1,
    }
}
