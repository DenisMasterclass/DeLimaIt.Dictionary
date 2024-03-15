using DeLimaIt.Dictionary.Application.Features.Configuration.Models;
using DeLimaIt.Dictionary.Application.Features.Configuration.Repository;
using DeLimaIt.Dictionary.Application.Features.Configuration.Validations;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using DelimaIt.Core.UseCases;
using DeLimaIt.Dictionary.Application.Features.Configuration.UseCase;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.DependencyInjection
{
    public static class ConfigurationExtension
    {
        public static IServiceCollection AddConfigurationParameters(this IServiceCollection services)
        {
            services.AddScoped<IConfigurationRepository, ConfigurationRepository>();

            services.AddSingleton<IValidator<ConfigurationParameterGetInput>, ConfigurationParameterGetValidation>();
            services.AddSingleton<IValidator<ConfigurationParameterInput>, ConfigurationParameterValuesValidation>();
            services.AddSingleton<IValidator<ConfigurationParameterValueGetInput>, ConfigurationParameterValueGetValidation>();

            services.AddScoped<UseCaseHandlerBase<ConfigurationParameterGetInput, List<ConfigurationParameterGetOutput>>, ConfigurationGetParameterUseCase>();
            services.AddScoped<UseCaseHandlerBase<ConfigurationParameterValueGetInput, List<ConfigurationParameterValueGetOutput>>, ConfigurationGetParameterValueUseCase>();
            services.AddScoped<UseCaseHandlerBase<ConfigurationParameterInput, ConfigurationParameterOutput>, ConfigurationParameterValuesUseCase>();

            return services;
        }
    }
}
