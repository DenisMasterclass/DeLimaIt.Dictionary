namespace DeLimaIt.Dictionary.Application.Features.Configuration.Models
{
    public sealed class ConfigurationParameterValueGetInput
    {
        public ConfigurationParameterValueGetInput(int parameterId)
        {
            ParameterId = parameterId;
        }

        public int ParameterId { get; set; }
    }
}
