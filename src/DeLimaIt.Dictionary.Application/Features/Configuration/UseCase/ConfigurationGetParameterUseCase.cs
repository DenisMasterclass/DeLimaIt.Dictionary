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
    public sealed class ConfigurationGetParameterUseCase
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ILogger _logger;

        public ConfigurationGetParameterUseCase(IConfigurationRepository configurationRepository,IValidator<ConfigurationParameterGetInput> validator, ILogger<ConfigurationGetParameterUseCase> logger) 
        {
            _configurationRepository = configurationRepository;
            _logger = logger;
        }
        public async Task<List<ConfigurationParameterGetOutput>> GetConfiguration(ConfigurationParameterGetInput request, CancellationToken cancellationToken)
        {
            var output = new List<ConfigurationParameterGetOutput>();
            var filter = new ConfigurationParameterFilter(request.ModuleId);
            try
            {
                var parametersModelList = await _configurationRepository.GetParametersAsync(filter, cancellationToken);
                var parameterList = parametersModelList.Select(c => new ConfigurationParameterGetOutput(c.Id, c.Name)).ToList();
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
