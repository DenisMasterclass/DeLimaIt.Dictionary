using DelimaIt.Core.UseCases;
using DelimaIt.Core.UseCases.Outputs;
using DeLimaIt.Dictionary.Application.Features.Configuration.Models;
using Microsoft.Extensions.Logging;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.UseCase
{
    public class ConfigurationGetModuleUseCase : UseCaseHandlerBase<ConfigurationModuleInput, List<ConfigurationModuleOutput>>
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ILogger<ConfigurationGetModuleUseCase> _logger;
        public ConfigurationGetModuleUseCase(IConfigurationRepository configurationRepository, ILogger<ConfigurationGetModuleUseCase> logger) : base(logger)
        {
            _configurationRepository = configurationRepository;
            _logger = logger;
        }
        protected override async Task<Output<List<ConfigurationModuleOutput>>> HandleAsync(ConfigurationModuleInput request, CancellationToken cancellationToken)
        {
            var output = new Output<List<ConfigurationModuleOutput>>();
            var ModuleModelList = await _configurationRepository.GetModulesAsync(cancellationToken);
            output.AddResult(ModuleModelList);

            return output;
        }
    }
}
