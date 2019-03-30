using System;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Job.FinancesAlerts.AzureRepositories
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class SmsMessageMockEntity : AzureTableEntity
    {
        public static string GeneratePartitionKey(string phoneNumber)
        {
            return phoneNumber;
        }

        public string Id => RowKey;

        public string PhoneNumber => PartitionKey;

        private DateTime _dateTime;
        public DateTime DateTime
        {
            get => _dateTime;
            set
            {
                if (_dateTime != value)
                {
                    _dateTime = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

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
