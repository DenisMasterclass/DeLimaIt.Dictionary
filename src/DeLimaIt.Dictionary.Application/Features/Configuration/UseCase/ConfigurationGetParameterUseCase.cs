using DeLimaIt.Dictionary.Application.Features.Configuration.Models;
using DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using DelimaIt.Core.UseCases;
using DelimaIt.Core.UseCases.Outputs;
namespace DeLimaIt.Dictionary.Application.Features.Configuration.UseCase
{
    public sealed class ConfigurationGetParameterUseCase : UseCaseHandlerBase<ConfigurationParameterGetInput, List<ConfigurationParameterGetOutput>>
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ILogger _logger;

        public ConfigurationGetParameterUseCase(IConfigurationRepository configurationRepository, IValidator<ConfigurationParameterGetInput> validator, ILogger<ConfigurationGetParameterUseCase> logger)
        {
            _configurationRepository = configurationRepository;
            _logger = logger;
        }
        protected override async Task<Output<List<ConfigurationParameterGetOutput>>> HandleAsync (ConfigurationParameterGetInput request, CancellationToken cancellationToken)
        {
            var output = new Output<List<ConfigurationParameterGetOutput>>();
            var filter = new ConfigurationParameterFilter(request.ModuleId);
            var parametersModelList = await _configurationRepository.GetParametersAsync(filter, cancellationToken);
            var parameterList = parametersModelList.Select(c => new ConfigurationParameterGetOutput(c.Id, c.Name)).ToList();
            output.AddResult(parameterList);

            return output;
        }
    }
}
