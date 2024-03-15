using DelimaIt.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.Models
{
    public sealed class ConfigurationGetInput : IRequest<ConfigurationGetOutput>
    {
        public ConfigurationGetInput(int moduleId, string parameterId)
        {
            ModuleId = moduleId;
            ParameterId = parameterId;
        }

        public ConfigurationGetInput(int moduleId, string parameterId, Guid correlationId, Guid transactionId) : this(moduleId, parameterId)
        {
            CorrelationId = correlationId;
            TransactionId = transactionId;
        }

        public int ModuleId { get; set; }
        public string ParameterId { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid TransactionId { get; set; }

    }
}
