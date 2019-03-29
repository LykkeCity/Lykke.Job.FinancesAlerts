using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Job.FinancesAlerts.AzureRepositories
{
    public class SmsMessageMockEntity : TableEntity
    {
        public static string GeneratePartitionKey(string phoneNumber)
        {
            return phoneNumber;
        }

        public string Id => RowKey;

        public string PhoneNumber => PartitionKey;

        public DateTime DateTime { get; set; }

        public string From { get; set; }

        public string Text { get; set; }

        public static SmsMessageMockEntity Create(string phoneNumber, string from, string message)
        {
            return new SmsMessageMockEntity
            {
                PartitionKey = GeneratePartitionKey(phoneNumber),
                RowKey = Guid.NewGuid().ToString(),
                DateTime = DateTime.UtcNow,
                Text = message,
                From = from,
            };
        }
    }
}
