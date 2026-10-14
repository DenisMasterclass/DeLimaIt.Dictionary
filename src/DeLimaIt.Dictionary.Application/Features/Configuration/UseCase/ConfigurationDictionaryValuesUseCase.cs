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
            var DictionaryValue = new DictionaryValueEntity(request.DictionaryId, request.OldKey, request.Key, request.Value);
            int rowsAffected;
            string operationName;
            switch (request.Operation)
            {
                case OperationType.Insert:
                    rowsAffected = await _configurationRepository.InsertDictionaryValue(DictionaryValue, cancellationToken);
                    operationName = "Insert";
                    break;
                case OperationType.Update:
                    rowsAffected = await _configurationRepository.UpdateDictionaryValue(DictionaryValue, cancellationToken);
                    operationName = "Update";
                    break;
                case OperationType.Delete:
                    rowsAffected = await _configurationRepository.DeleteDictionaryValue(DictionaryValue, cancellationToken);
                    operationName = "Delete";
                    break;
                default:
                    output.AddErrorMessages("Unsupported dictionary operation.");
                    return output;
            }

            _logger.LogInformation("{Operation} operation. Rows Affected: {RowsAffected}", operationName, rowsAffected);
            output.AddResult(new ConfigurationDictionaryOutput
            {
                RecordsAffect = rowsAffected,
                Message = $"{operationName} completed."
            });
            return output;
        }
    }
}
