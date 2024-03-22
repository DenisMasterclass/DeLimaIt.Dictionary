using DeLimaIt.Dictionary.Application.Features.Configuration.Models;
using DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using DelimaIt.Core.UseCases;
using DelimaIt.Core.UseCases.Outputs;
namespace DeLimaIt.Dictionary.Application.Features.Configuration.UseCase
{
    public sealed class ConfigurationGetDictionaryUseCase : UseCaseHandlerBase<ConfigurationDictionaryGetInput, List<ConfigurationDictionaryGetOutput>>
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ILogger<ConfigurationGetDictionaryUseCase> _logger;

        public ConfigurationGetDictionaryUseCase(IConfigurationRepository configurationRepository, IValidator<ConfigurationDictionaryGetInput> validator, ILogger<ConfigurationGetDictionaryUseCase> logger)
        :base(logger, validator)
        {
            _configurationRepository = configurationRepository;
            _logger = logger;
        }
        protected override async Task<Output<List<ConfigurationDictionaryGetOutput>>> HandleAsync (ConfigurationDictionaryGetInput request, CancellationToken cancellationToken)
        {
            var output = new Output<List<ConfigurationDictionaryGetOutput>>();
            var filter = new ConfigurationDictionaryFilter(request.ModuleId);
            var DictionariesModelList = await _configurationRepository.GetDictionariesAsync(filter, cancellationToken);
            var DictionaryList = DictionariesModelList.Select(c => new ConfigurationDictionaryGetOutput(c.Id, c.Name)).ToList();
            output.AddResult(DictionaryList);

            return output;
        }
    }
}
