using DeLimaIt.Dictionary.Application.Features.Configuration.Models;
using DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities;

namespace DeLimaIt.Dictionary.Application.Features.Configuration
{
    public interface IConfigurationRepository
    {
        Task<IEnumerable<DictionaryEntity>> GetDictionariesAsync(ConfigurationDictionaryFilter filter, CancellationToken cancellationToken = default);
        Task<List<DictionaryValueEntity>> GetDictionariesValuesAsync(DictionaryFilter filter, CancellationToken cancellationToken = default);
        Task<int> InsertDictionaryValue(DictionaryValueEntity DictionaryValueEntity, CancellationToken cancellationToken = default);
        Task<int> UpdateDictionaryValue(DictionaryValueEntity DictionaryValueEntity, CancellationToken cancellationToken = default);
        Task<int> DeleteDictionaryValue(DictionaryValueEntity DictionaryValueEntity, CancellationToken cancellationToken = default);
        Task<Dictionary<string, Dictionary<string, string>>> GetAllDictionaries(int moduleId, CancellationToken cancellationToken = default);
        Task<List<ConfigurationModuleOutput>> GetModulesAsync(CancellationToken cancellationToken = default);
        Task<int> InsertModule(ConfigurationModuleEntity configurationModuleEntity, CancellationToken cancellationToken = default);
        Task<int> UpdateModule(ConfigurationModuleEntity configurationModuleEntity, CancellationToken cancellationToken = default);
        Task<int> DeleteModule(ConfigurationModuleEntity configurationModuleEntity, CancellationToken cancellationToken = default);
       

    }
}