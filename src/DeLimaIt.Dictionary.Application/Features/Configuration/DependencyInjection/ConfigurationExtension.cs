
using DeLimaIt.Dictionary.Application.Features.Configuration.Models;
using DeLimaIt.Dictionary.Application.Features.Configuration.Repository;
using DeLimaIt.Dictionary.Application.Features.Configuration.Validations;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.DependencyInjection
{
    public static class ConfigurationExtension
    {
        public static IServiceCollection AddConfigurationParameters(this IServiceCollection services)
        {
            services.AddScoped<IConfigurationRepository, ConfigurationRepository>();

            services.AddSingleton<IValidator<ConfigurationParameterGetInput>, ConfigurationParameterGetValidation>();
           

        }
    }
}
