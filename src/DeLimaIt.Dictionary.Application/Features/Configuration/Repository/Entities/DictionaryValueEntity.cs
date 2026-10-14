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
            this.DictionaryId = DictionaryId;
            Key = key;
            OldKey = key;
            Value = value;
        }

        public DictionaryValueEntity(int dictionaryId, string oldKey, string key, string value)
        {
            DictionaryId = dictionaryId;
            OldKey = oldKey;
            Key = key;
            Value = value;
        }

        public int DictionaryId { get; set; }
        public string Key { get; set; } = string.Empty;
        public string OldKey { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
