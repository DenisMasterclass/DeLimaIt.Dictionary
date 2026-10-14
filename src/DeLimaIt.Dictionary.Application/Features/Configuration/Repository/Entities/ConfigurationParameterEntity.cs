namespace DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities;

public sealed class ConfigurationParameterEntity
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public string Name { get; set; } = string.Empty;
}
