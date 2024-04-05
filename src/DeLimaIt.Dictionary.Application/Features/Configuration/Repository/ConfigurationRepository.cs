using Dapper;
using DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities;
using DeLimaIt.Dictionary.Application.Shared;
using static DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Sql.ConfigurationRepositorySQL;
namespace DeLimaIt.Dictionary.Application.Features.Configuration.Repository
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly DataContext _context;
        public ConfigurationRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<DictionaryEntity>> GetDictionariesAsync(ConfigurationDictionaryFilter filter, CancellationToken cancellationToken = default)
        {
            var connection = _context.Connection;
            var configurationModelList = await connection.QueryAsync<DictionaryEntity>(GetSqlDictionaries,filter,_context.Transaction).ConfigureAwait(false);
            return configurationModelList;
        }
        public async Task<List<DictionaryValueEntity>> GetDictionariesValuesAsync(DictionaryFilter filter,CancellationToken cancellationToken = default)
        {
            var connection = _context.Connection;
            var DictionaryValueList = await connection.QueryAsync<DictionaryValueEntity>(GetSqlDictionariesValues,filter,_context.Transaction).ConfigureAwait(false);
            return DictionaryValueList.ToList();
        }
        public async Task<int> InsertDictionaryValue(DictionaryValueEntity DictionaryValueEntity, CancellationToken cancellationToken = default)
        {
            var connection = _context.Connection;
            var rowsAffected = await connection.ExecuteAsync(InsertSqlDictionaryValue, DictionaryValueEntity, _context.Transaction).ConfigureAwait(false);
            return rowsAffected;
        }
        public async Task<int> UpdateDictionaryValue(DictionaryValueEntity DictionaryValueEntity, CancellationToken cancellationToken = default)
        {
            var connection = _context.Connection;
            var rowsAffected = await connection.ExecuteAsync(UpdateSqlDictionaryValue, DictionaryValueEntity, _context.Transaction).ConfigureAwait(false);
            return rowsAffected;
        }
        public async Task<int> DeleteDictionaryValue(DictionaryValueEntity DictionaryValueEntity, CancellationToken cancellationToken = default)
        {
            var connection = _context.Connection;
            var rowsAffected = await connection.ExecuteAsync(DeleteSqlDictionaryValue, DictionaryValueEntity, _context.Transaction).ConfigureAwait(false);
            return rowsAffected;
        }
        public async Task<Dictionary<string,Dictionary<string,string>>> GetAllDictionaries (int moduleId, CancellationToken cancellationToken = default)
        {
            Dictionary<string, Dictionary<string, string>> dicDictionaries = new();
            var Dictionaries = await this.GetDictionariesAsync(new ConfigurationDictionaryFilter(moduleId), cancellationToken);
            foreach (var Dictionary in Dictionaries)
            {
                Dictionary<string, string> dicDictionariesValues = new();
                dicDictionaries[Dictionary.Name] = dicDictionariesValues;
                var DictionariesValues = await this.GetDictionariesValuesAsync(new DictionaryFilter(Dictionary.Id),cancellationToken);
                foreach (var DictionaryValue in DictionariesValues)
                {
                    dicDictionariesValues[DictionaryValue.Key] = DictionaryValue.Value;
                }
            }
            return dicDictionaries;
        }
        public async Task<IEnumerable<ConfigurationModuleEntity>> GetModulesAsync(ConfigurationDictionaryFilter filter, CancellationToken cancellationToken = default)
        {
            var connection = _context.Connection;
            var configurationModelList = await connection.QueryAsync<ConfigurationModuleEntity>(SelectSqlModules, filter, _context.Transaction).ConfigureAwait(false);
            return configurationModelList;
        }
        public async Task<int> InsertModule(ConfigurationModuleEntity configurationModuleEntity, CancellationToken cancellationToken = default)
        {
            var connection = _context.Connection;
            var rowsAffected = await connection.ExecuteAsync(InsertSqlModules, configurationModuleEntity, _context.Transaction).ConfigureAwait(false);
            return rowsAffected;
        }
        public async Task<int> UpdateModule(ConfigurationModuleEntity configurationModuleEntity, CancellationToken cancellationToken = default)
        {
            var connection = _context.Connection;
            var rowsAffected = await connection.ExecuteAsync(UpdateSqlModules, configurationModuleEntity, _context.Transaction).ConfigureAwait(false);
            return rowsAffected;
        }
        public async Task<int> DeleteModule(ConfigurationModuleEntity configurationModuleEntity, CancellationToken cancellationToken = default)
        {
            var connection = _context.Connection;
            var rowsAffected = await connection.ExecuteAsync(DeleteSqlModules, configurationModuleEntity, _context.Transaction).ConfigureAwait(false);
            return rowsAffected;
        }
    }
}
