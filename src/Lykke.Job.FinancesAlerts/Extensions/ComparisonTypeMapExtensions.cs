using System;
using Lykke.Job.FinancesAlerts.Domain;

namespace Lykke.Job.FinancesAlerts.Extensions
{
    internal static class ComparisonTypeMapExtensions
    {
        internal static Client.Models.ComparisonType ToClient(this ComparisonType comparisonType)
        {
            switch (comparisonType)
            {
                case ComparisonType.GreaterThan:
                    return Client.Models.ComparisonType.GreaterThan;
                case ComparisonType.GreaterOrEqual:
                    return Client.Models.ComparisonType.GreaterOrEqual;
                case ComparisonType.Equal:
                    return Client.Models.ComparisonType.Equal;
                case ComparisonType.LessThan:
                    return Client.Models.ComparisonType.LessThan;
                case ComparisonType.LessOrEqual:
                    return Client.Models.ComparisonType.LessOrEqual;
                default:
                    throw new ArgumentOutOfRangeException(nameof(comparisonType), comparisonType, null);
            }
        }

        internal static ComparisonType ToDomain(this Client.Models.ComparisonType comparisonType)
        {
            switch (comparisonType)
            {
                case Client.Models.ComparisonType.GreaterThan:
                    return ComparisonType.GreaterThan;
                case Client.Models.ComparisonType.GreaterOrEqual:
                    return ComparisonType.GreaterOrEqual;
                case Client.Models.ComparisonType.Equal:
                    return ComparisonType.Equal;
                case Client.Models.ComparisonType.LessThan:
                    return ComparisonType.LessThan;
                case Client.Models.ComparisonType.LessOrEqual:
                    return ComparisonType.LessOrEqual;
                default:
                    throw new ArgumentOutOfRangeException(nameof(comparisonType), comparisonType, null);
            }
        }
    }
}
