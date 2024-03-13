namespace DeLimaIt.Dictionary.Application.Features.Configuration.Models
{
    public sealed class ConfigurationGetOutput
    {
        public int ModuleId { get; set; }
        public string ParameterId { get; set; }
        public IList<ParameterGetOutput> Parameters { get; set; }
    }
}
