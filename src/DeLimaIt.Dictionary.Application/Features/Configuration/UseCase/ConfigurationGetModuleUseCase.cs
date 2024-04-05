using DelimaIt.Core.UseCases;
using DelimaIt.Core.UseCases.Outputs;
using DeLimaIt.Dictionary.Application.Features.Configuration.Models;
using DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var filter = new ConfigurationDictionaryFilter(request.Entity.ModuleId);
            var ModuleModelList = await _configurationRepository.GetModulesAsync(filter, cancellationToken);
            output.AddResult((List<ConfigurationModuleOutput>)ModuleModelList);

            return output;
        }
    }
}
