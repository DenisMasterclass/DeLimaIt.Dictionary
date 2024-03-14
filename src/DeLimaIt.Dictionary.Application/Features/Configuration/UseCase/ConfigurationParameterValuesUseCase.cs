using DeLimaIt.Dictionary.Application.Features.Configuration.Models;
using DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.UseCase
{
    public sealed class ConfigurationParameterValuesUseCase
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ILogger<ConfigurationParameterValuesUseCase> _logger;

        public ConfigurationParameterValuesUseCase(IConfigurationRepository configurationRepository, IValidator<ConfigurationParameterInput> validator, ILogger<ConfigurationParameterValuesUseCase> logger)
        {
            _configurationRepository = configurationRepository;
            _logger = logger;
        }

        protected async Task<ConfigurationParameterOutput> ParametersValueOperations(ConfigurationParameterInput request, CancellationToken cancellationToken)
        {
            var output = new ConfigurationParameterOutput();
            var parameterValue = new ParameterValueEntity(request.ParameterId, request.Key, request.Value);
            if (request.Operation == OperationType.Insert)
            {
                var rowsAffected = _configurationRepository.InsertParameterValue(parameterValue, cancellationToken);
                _logger.LogInformation("Insert operation. Rows Affected :" + rowsAffected);
                output.RecordsAffect = rowsAffected.Result;
                output.Message = "Insert operation. Rows Affected :" + rowsAffected;
            }
            if (request.Operation == OperationType.Update)
            {
                var rowsAffected = _configurationRepository.UpdateParameterValue(parameterValue, cancellationToken);
                _logger.LogInformation("Update operation. Rows Affected :" + rowsAffected);
                output.RecordsAffect = rowsAffected.Result;
                output.Message = "Update operation. Rows Affected :" + rowsAffected;
            }
            if (request.Operation == OperationType.Delete)
            {
                var rowsAffected = _configurationRepository.DeleteParameterValue(parameterValue, cancellationToken);
                _logger.LogInformation("Delete operation. Rows Affected :" + rowsAffected);
                output.RecordsAffect = rowsAffected.Result;
                output.Message = "Delete operation. Rows Affected :" + rowsAffected;
            }

            return output;
        }
    }
}
