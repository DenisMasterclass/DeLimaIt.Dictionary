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
        public async Task<IEnumerable<ParameterEntity>> GetParametersAsync(ConfigurationParameterFilter filter, CancellationToken cancellationToken = default)
        {
            var connection = _context.Connection;
            var configurationModelList = await connection.QueryAsync<ParameterEntity>(GetSqlParameters,filter,_context.Transaction).ConfigureAwait(false);
            return configurationModelList;
        }
        public async Task<List<ParameterValueEntity>> GetParametersValuesAsync(ParameterFilter filter,CancellationToken cancellationToken = default)
        {
            var connection = _context.Connection;
            var parameterValueList = await connection.QueryAsync<ParameterValueEntity>(GetSqlParametersValues,filter,_context.Transaction).ConfigureAwait(false);
            return parameterValueList.ToList();
        }
        public async Task<int> InsertParameterValue(ParameterValueEntity parameterValueEntity, CancellationToken cancellationToken = default)
        {
            var connection = _context.Connection;
            var rowsAffected = await connection.ExecuteAsync(InsertSqlParameterValue, parameterValueEntity, _context.Transaction).ConfigureAwait(false);
            return rowsAffected;
        }
        public async Task<int> UpdateParameterValue(ParameterValueEntity parameterValueEntity, CancellationToken cancellationToken = default)
        {
            var connection = _context.Connection;
            var rowsAffected = await connection.ExecuteAsync(UpdateSqlParameterValue, parameterValueEntity, _context.Transaction).ConfigureAwait(false);
            return rowsAffected;
        }
        public async Task<int> DeleteParameterValue(ParameterValueEntity parameterValueEntity, CancellationToken cancellationToken = default)
        {
            var connection = _context.Connection;
            var rowsAffected = await connection.ExecuteAsync(DeleteSqlParameterValue, parameterValueEntity, _context.Transaction).ConfigureAwait(false);
            return rowsAffected;
        }
        public async Task<Dictionary<string,Dictionary<string,string>>> GetAllParameters (int moduleId, CancellationToken cancellationToken = default)
        {
            Dictionary<string, Dictionary<string, string>> dicParameters = new();
            var parameters = await this.GetParametersAsync(new ConfigurationParameterFilter(moduleId), cancellationToken);
            foreach (var parameter in parameters)
            {
                Dictionary<string, string> dicParametersValues = new();
                dicParameters[parameter.Name] = dicParametersValues;
                var parametersValues = await this.GetParametersValuesAsync(new ParameterFilter(parameter.Id),cancellationToken);
                foreach (var parameterValue in parametersValues)
                {
                    dicParametersValues[parameterValue.Key] = parameterValue.Value;
                }
            }
            return dicParameters;
        }
    }
}
