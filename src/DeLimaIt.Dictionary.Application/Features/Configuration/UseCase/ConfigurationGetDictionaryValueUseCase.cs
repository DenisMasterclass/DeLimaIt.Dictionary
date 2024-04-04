using DelimaIt.Core.UseCases;
using DeLimaIt.Dictionary.Application.Features.Configuration.Models;
using DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities;
using FluentValidation;
using DelimaIt.Core.UseCases.Outputs;
using Microsoft.Extensions.Logging;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.UseCase
{
    public sealed class ConfigurationGetDictionaryValueUseCase : UseCaseHandlerBase<ConfigurationDictionaryValueGetInput, List<ConfigurationDictionaryValueGetOutput>>
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ILogger<ConfigurationGetDictionaryValueUseCase> _logger;

        public ConfigurationGetDictionaryValueUseCase(IConfigurationRepository configurationRepository, IValidator<ConfigurationDictionaryValueGetInput> validator, ILogger<ConfigurationGetDictionaryValueUseCase> logger)
        :base(logger,validator)
        {
            _configurationRepository = configurationRepository;
            _logger = logger;
        }
        protected override async Task<Output<List<ConfigurationDictionaryValueGetOutput>>> HandleAsync (ConfigurationDictionaryValueGetInput request, CancellationToken cancellationToken)
        {
            var output = new Output<List<ConfigurationDictionaryValueGetOutput>>();
            var filter = new DictionaryFilter(request.DictionaryId);
                var DictionariesModelList = await _configurationRepository.GetDictionariesValuesAsync(filter, cancellationToken);
                var DictionaryList = DictionariesModelList.Select(c => new ConfigurationDictionaryValueGetOutput(c.Key, c.Value)).ToList();
                output.AddResult(DictionaryList);

            return output;
        }
    }
}
