using DelimaIt.Core.UseCases;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.Models
{
    public sealed class ConfigurationParameterValueGetInput : IRequest<List<ConfigurationParameterValueGetOutput>>
    {
        public ConfigurationParameterValueGetInput(int parameterId)
        {
            ParameterId = parameterId;
        }

        public ConfigurationParameterValueGetInput(int parameterId, Guid correlationId, Guid transactionId) : this(parameterId)
        {
            CorrelationId = correlationId;
            TransactionId = transactionId;
        }

        public int ParameterId { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid TransactionId { get; set; }
    }
}
