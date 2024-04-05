using System.ComponentModel.DataAnnotations;

namespace DeLimaIt.Dictionary.Api.Transport.V1
{
    public sealed class ModuleResponse
    {
        public ModuleResponse()
        {
                
        }

        public ModuleResponse(int id, string name)
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
