using System.ComponentModel.DataAnnotations;

namespace DeLimaIt.Dictionary.Api.Transport.V1
{
    public sealed class DictionaryResponse
    {
        public DictionaryResponse()
        {
                
        }

        public DictionaryResponse(int id, string name)
        {
            Id = id;
            Name = name;
        }

        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
