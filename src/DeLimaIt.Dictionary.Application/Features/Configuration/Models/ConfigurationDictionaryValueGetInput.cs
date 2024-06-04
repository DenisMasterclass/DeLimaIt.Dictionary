using DelimaIt.Core.UseCases;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.Models
{
    public sealed class ConfigurationDictionaryValueGetInput : IRequest<List<ConfigurationDictionaryValueGetOutput>>
    {
        public ConfigurationDictionaryValueGetInput(int dictionaryId)
        {
            DictionaryId = dictionaryId;
        }

        public ConfigurationDictionaryValueGetInput(int DictionaryId, Guid correlationId, Guid transactionId) : this(DictionaryId)
        {
            CorrelationId = correlationId == Guid.Empty ? Guid.NewGuid() : correlationId;
            TransactionId = transactionId == Guid.Empty ? Guid.NewGuid() : transactionId;
        }

        public int DictionaryId { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid TransactionId { get; set; }
    }
}
