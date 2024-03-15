using System.ComponentModel.DataAnnotations;

namespace DeLimaIt.Dictionary.Api.Transport.V1
{
    public sealed class ParameterValueRequest
    {
        public ParameterValueRequest()
        {
                
        }

        public ParameterValueRequest(string key, string value)
        {
            Key = key;
            Value = value;
        }

        [Required]
        public string Key { get; set; }
        [Required]
        public string Value { get; set; }
    }
}
