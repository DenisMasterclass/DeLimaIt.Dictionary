using DelimaIt.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.Models
{
    public sealed class ConfigurationParameterGetInput : IRequest<List<ConfigurationParameterGetOutput>>
    {
        public ConfigurationParameterGetInput(int moduleId)
        {
            ModuleId = moduleId;
        }

        public ConfigurationParameterGetInput(int moduleId, Guid correlationId, Guid transactionId) : this(moduleId)
        {
            CorrelationId = correlationId;
            TransactionId = transactionId;
        }

        public int ModuleId { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid TransactionId { get; set; }
    }
}
