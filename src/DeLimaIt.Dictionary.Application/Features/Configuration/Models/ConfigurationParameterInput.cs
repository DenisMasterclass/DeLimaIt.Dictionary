using DelimaIt.Core.UseCases;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.Models
{
    public enum OperationType: int
    {
        Insert,
        Update,
        Delete
    }
    public sealed class ConfigurationParameterInput : IRequest<ConfigurationParameterOutput>
    {
        public ConfigurationParameterInput(OperationType operation, int parameterId, string key, string value)
        {
            Operation = operation;
            ParameterId = parameterId;
            Key = key;
            Value = value;
        }

        public ConfigurationParameterInput(OperationType operation, int parameterId, string key, string value, Guid correlationId, Guid transactionId) : this(operation, parameterId, key, value)
        {
            CorrelationId = correlationId == Guid.Empty ? Guid.NewGuid() : correlationId;
            TransactionId = transactionId == Guid.Empty ? Guid.NewGuid() : transactionId;
        }

        public OperationType Operation { get; set; }
        public int ParameterId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid TransactionId { get; set; }
    }
}
