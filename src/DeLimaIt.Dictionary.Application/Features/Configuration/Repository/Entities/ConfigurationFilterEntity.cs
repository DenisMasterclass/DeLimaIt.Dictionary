using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities
{
    public class ConfigurationFilterEntity
    {
        public ConfigurationFilterEntity()
        {

        }

        public ConfigurationFilterEntity(int moduleId, string parameterId)
        {
            ModuleId = moduleId;
            ParameterId = parameterId;
        }

        public int ModuleId { get; set; }
        public string ParameterId { get; set; }
    }
}
