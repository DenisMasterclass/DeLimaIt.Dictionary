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
        public ConfigurationGetInput(int moduleId, string DictionaryId)
        {
            ModuleId = moduleId;
            DictionaryId = DictionaryId;
        }

        public ConfigurationGetInput(int moduleId, string DictionaryId, Guid correlationId, Guid transactionId) : this(moduleId, DictionaryId)
        {
            CorrelationId = correlationId == Guid.Empty ? Guid.NewGuid() : correlationId;
            TransactionId = transactionId == Guid.Empty ? Guid.NewGuid() : transactionId;
        }

        public int ModuleId { get; set; }
        public string DictionaryId { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid TransactionId { get; set; }

    }
}
