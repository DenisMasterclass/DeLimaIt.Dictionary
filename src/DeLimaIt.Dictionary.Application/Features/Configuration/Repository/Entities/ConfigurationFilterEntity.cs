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

        public ConfigurationFilterEntity(int moduleId, string DictionaryId)
        {
            ModuleId = moduleId;
            DictionaryId = DictionaryId;
        }

        public int ModuleId { get; set; }
        public string DictionaryId { get; set; }
    }
}
