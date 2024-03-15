using DelimaIt.Core.UseCases;
using DeLimaIt.Dictionary.Application.Features.Configuration.Models;
using DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities;
using FluentValidation;
using DelimaIt.Core.UseCases.Outputs;
using Microsoft.Extensions.Logging;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.UseCase
{
    public sealed class ConfigurationGetParameterValueUseCase : UseCaseHandlerBase<ConfigurationParameterValueGetInput, List<ConfigurationParameterValueGetOutput>>
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ILogger<ConfigurationGetParameterValueUseCase> _logger;

        public ConfigurationGetParameterValueUseCase(IConfigurationRepository configurationRepository, IValidator<ConfigurationParameterValueGetInput> validator, ILogger<ConfigurationGetParameterValueUseCase> logger)
        :base(logger,validator)
        {
            _configurationRepository = configurationRepository;
            _logger = logger;
        }
        protected override async Task<Output<List<ConfigurationParameterValueGetOutput>>> HandleAsync (ConfigurationParameterValueGetInput request, CancellationToken cancellationToken)
        {
            var output = new Output<List<ConfigurationParameterValueGetOutput>>();
            var filter = new ParameterFilter(request.ParameterId);
                var parametersModelList = await _configurationRepository.GetParametersValuesAsync(filter, cancellationToken);
                var parameterList = parametersModelList.Select(c => new ConfigurationParameterValueGetOutput(c.Key, c.Value)).ToList();
                output.AddResult(parameterList);

            return output;
        }
    }
}
