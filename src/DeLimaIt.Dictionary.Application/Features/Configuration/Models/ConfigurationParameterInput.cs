using DelimaIt.Core.UseCases;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.Models
{
    public enum OperationType: int
    {
        Insert,
        Update,
        Delete
    }
    public sealed class ConfigurationDictionaryInput : IRequest<ConfigurationDictionaryOutput>
    {
        public ConfigurationDictionaryInput(OperationType operation, int DictionaryId, string key, string value)
        {
            Operation = operation;
            DictionaryId = DictionaryId;
            Key = key;
            Value = value;
        }

        public ConfigurationDictionaryInput(OperationType operation, int DictionaryId, string key, string value, Guid correlationId, Guid transactionId) : this(operation, DictionaryId, key, value)
        {
            CorrelationId = correlationId == Guid.Empty ? Guid.NewGuid() : correlationId;
            TransactionId = transactionId == Guid.Empty ? Guid.NewGuid() : transactionId;
        }

        public OperationType Operation { get; set; }
        public int DictionaryId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid TransactionId { get; set; }
    }
}
