

using DelimaIt.Core.UseCases;
using DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.Models
{
    public class ConfigurationModuleInput : IRequest<ConfigurationModuleOutput>
    {
        public ConfigurationModuleInput(ConfigurationModuleEntity entity)
        {
            Entity = entity;            
        }
        public ConfigurationModuleInput(ConfigurationModuleEntity entity, Guid correlationId, Guid transactionId) : this(entity)
        {
            CorrelationId = correlationId == Guid.Empty ? Guid.NewGuid() : correlationId;
            TransactionId = transactionId == Guid.Empty ? Guid.NewGuid() : transactionId;
        }

        public ConfigurationModuleEntity Entity { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid TransactionId { get; set; }
    }
}
