namespace DeLimaIt.Dictionary.Application.Features.Configuration.Models
{
    public sealed class ConfigurationGetOutput
    {
        public int ModuleId { get; set; }
        public string DictionaryId { get; set; }
        public IList<DictionaryGetOutput> Dictionaries { get; set; }
    }
}
