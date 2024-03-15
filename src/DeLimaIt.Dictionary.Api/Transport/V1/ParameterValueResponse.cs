using System.ComponentModel.DataAnnotations;

namespace DeLimaIt.Dictionary.Api.Transport.V1
{
    public class ParameterValueResponse
    {
        public ParameterValueResponse()
        {

        }

        public ParameterValueResponse(int key, string value)
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
