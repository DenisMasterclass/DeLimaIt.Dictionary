using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities
{
    public class ConfigurationParameterFilter
    {
        public ConfigurationParameterFilter()
        {
            
        }

        public ConfigurationParameterFilter(int moduleId)
        {
            ModuleId = moduleId;
        }

        public int ModuleId { get; set; }
    }
}
