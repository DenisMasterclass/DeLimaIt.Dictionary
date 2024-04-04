using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities
{
    public class DictionaryValueEntity
    {
        public DictionaryValueEntity()
        {

        }

        public DictionaryValueEntity(int DictionaryId, string key, string value)
        {
            DictionaryId = DictionaryId;
            Key = key;
            Value = value;
        }

        public int DictionaryId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
