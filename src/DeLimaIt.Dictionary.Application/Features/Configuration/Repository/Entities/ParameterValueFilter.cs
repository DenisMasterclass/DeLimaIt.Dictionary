using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities
{
    public class ParameterValueFilter
    {
        public ParameterValueFilter()
        {

        }

        public ParameterValueFilter(int parameterId, string key, string value)
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
