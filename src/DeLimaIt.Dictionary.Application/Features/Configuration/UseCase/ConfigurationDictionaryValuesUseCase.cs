using DelimaIt.Core.UseCases;
using DelimaIt.Core.UseCases.Outputs;
using DeLimaIt.Dictionary.Application.Features.Configuration.Models;
using DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.UseCase
{
    public sealed class ConfigurationDictionaryValuesUseCase : UseCaseHandlerBase<ConfigurationDictionaryInput, ConfigurationDictionaryOutput>
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ILogger<ConfigurationDictionaryValuesUseCase> _logger;

        public ConfigurationDictionaryValuesUseCase(IConfigurationRepository configurationRepository, IValidator<ConfigurationDictionaryInput> validator, ILogger<ConfigurationDictionaryValuesUseCase> logger)
        :base(logger, validator) 
        {
            _configurationRepository = configurationRepository;
            _logger = logger;
        }

        protected override async Task<Output<ConfigurationDictionaryOutput>> HandleAsync (ConfigurationDictionaryInput request, CancellationToken cancellationToken)
        {
            var output = new Output<ConfigurationDictionaryOutput>();
            var DictionaryValue = new DictionaryValueEntity(request.DictionaryId, request.Key, request.Value);
            if (request.Operation == OperationType.Insert)
            {
                var rowsAffected = _configurationRepository.InsertDictionaryValue(DictionaryValue, cancellationToken);
                _logger.LogInformation("Insert operation. Rows Affected :" + rowsAffected);
            }
            if (request.Operation == OperationType.Update)
            {
                var rowsAffected = _configurationRepository.UpdateDictionaryValue(DictionaryValue, cancellationToken);
                _logger.LogInformation("Update operation. Rows Affected :" + rowsAffected);
            }
            if (request.Operation == OperationType.Delete)
            {
                var rowsAffected = _configurationRepository.DeleteDictionaryValue(DictionaryValue, cancellationToken);
                _logger.LogInformation("Delete operation. Rows Affected :" + rowsAffected);
            }
            return output;
        }
    }
}
