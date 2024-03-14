using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities
{
    public class ParameterValueEntity
    {
        public ParameterValueEntity()
        {

        }

        public ParameterValueEntity(int parameterId, string key, string value)
        {
            ParameterId = parameterId;
            Key = key;
            Value = value;
        }

        public int ParameterId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
