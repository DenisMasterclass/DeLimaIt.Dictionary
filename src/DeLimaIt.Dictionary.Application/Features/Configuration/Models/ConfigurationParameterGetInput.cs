using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.Models
{
    public sealed class ConfigurationParameterGetInput 
    {
        public ConfigurationParameterGetInput(int moduleId) 
        {
              ModuleId = moduleId;
        }
        public int ModuleId { get; set; }

    }
}
