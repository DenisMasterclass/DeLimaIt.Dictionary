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
        public async Task<IEnumerable<ParameterEntity>> GetParametersAsync(ConfigurationParameterFilter filter)
        {
            var connection = _context.Connection;
            var configurationModelList = await connection.QueryAsync<ParameterEntity>(GetSqlParameters,filter,_context.Transaction).ConfigureAwait(false);
            return configurationModelList;
        }
        //value entity 
        //dictionary de dictionary
    }
}
