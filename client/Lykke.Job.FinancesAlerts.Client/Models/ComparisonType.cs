using System;

namespace Lykke.Job.FinancesAlerts.Client.Models
{
    [Flags]
    public enum ComparisonType : byte
    {
        GreaterThan = 0,
        GreaterOrEqual = 1,
        Equal = 2,
        LessThan = 4,
        LessOrEqual = 8,
    }
}
