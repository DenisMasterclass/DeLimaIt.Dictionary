using System.ComponentModel.DataAnnotations;

namespace DeLimaIt.Dictionary.Api.Transport.V1
{
    public sealed class DictionaryValueRequest
    {
        public DictionaryValueRequest()
        {
                
        }

        public DictionaryValueRequest(string key, string value)
        {
            Key = key;
            Value = value;
        }

        [Required, StringLength(200, MinimumLength = 1)]
        public string Key { get; set; } = string.Empty;
        [StringLength(200, MinimumLength = 1)]
        public string? OldKey { get; set; }
        [Required]
        public string Value { get; set; } = string.Empty;
    }
}
