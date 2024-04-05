using System.ComponentModel.DataAnnotations;

namespace DeLimaIt.Dictionary.Api.Transport.V1
{
    public class DictionaryValueResponse
    {
        public DictionaryValueResponse()
        {

        }

        public DictionaryValueResponse(int key, string value)
        {
            Key = key;
            Value = value;
        }

        [Required]
        public int Key { get; set; }
        [Required]
        public string Value { get; set; }
    }
}
