using DeLimaIt.Dictionary.Application.Features.Configuration.Models;
using DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.UseCase
{
    public sealed class ConfigurationGetParameterValueUseCase
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ILogger _logger;

        public ConfigurationGetParameterValueUseCase(IConfigurationRepository configurationRepository,IValidator<ConfigurationParameterValueGetInput> validator, ILogger<ConfigurationGetParameterUseCase> logger) 
        {
            _configurationRepository = configurationRepository;
            _logger = logger;
        }
        public async Task<List<ConfigurationParameterValueGetOutput>> GetParameterValue(ConfigurationParameterValueGetInput request, CancellationToken cancellationToken)
        {
            var output = new List<ConfigurationParameterValueGetOutput>();
            var filter = new ParameterFilter(request.ParameterId);
            try
            {
                var parametersModelList = await _configurationRepository.GetParametersValuesAsync(filter, cancellationToken);
                var parameterList = parametersModelList.Select(c => new ConfigurationParameterValueGetOutput(c.Key, c.Value)).ToList();
                output = parameterList;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocorreu o erro : " + ex.Message);
                throw;
            }


            return output;
        }
    }
}
