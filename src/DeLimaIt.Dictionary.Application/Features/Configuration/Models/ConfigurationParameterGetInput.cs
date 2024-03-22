using DelimaIt.Core.UseCases;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.Models
{
    public sealed class ConfigurationDictionaryGetInput : IRequest<List<ConfigurationDictionaryGetOutput>>
    {
        public ConfigurationDictionaryGetInput(int moduleId)
        {
            ModuleId = moduleId;
        }

        public ConfigurationDictionaryGetInput(int moduleId, Guid correlationId, Guid transactionId) : this(moduleId)
        {
            CorrelationId = correlationId == Guid.Empty ? Guid.NewGuid() : correlationId;
            TransactionId = transactionId == Guid.Empty ? Guid.NewGuid() : transactionId;
        }

        public int ModuleId { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid TransactionId { get; set; }
    }
}
