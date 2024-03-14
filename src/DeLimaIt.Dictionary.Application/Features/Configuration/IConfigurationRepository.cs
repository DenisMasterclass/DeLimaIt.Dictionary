using DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities;

namespace DeLimaIt.Dictionary.Application.Features.Configuration
{
    public interface IConfigurationRepository
    {
        Task<IEnumerable<ParameterEntity>> GetParametersAsync(ConfigurationParameterFilter filter, CancellationToken cancellationToken = default);
        Task<List<ParameterValueEntity>> GetParametersValuesAsync(ParameterFilter filter, CancellationToken cancellationToken = default);
        Task<int> InsertParameterValue(ParameterValueEntity parameterValueEntity, CancellationToken cancellationToken = default);
        Task<int> UpdateParameterValue(ParameterValueEntity parameterValueEntity, CancellationToken cancellationToken = default);
        Task<int> DeleteParameterValue(ParameterValueEntity parameterValueEntity, CancellationToken cancellationToken = default);
        Task<Dictionary<string, Dictionary<string, string>>> GetAllParameters(int moduleId, CancellationToken cancellationToken = default);


    }
}