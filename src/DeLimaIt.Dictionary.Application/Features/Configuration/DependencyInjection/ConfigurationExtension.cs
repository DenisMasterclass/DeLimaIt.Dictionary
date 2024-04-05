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
        public static IServiceCollection AddConfigurationDictionaries(this IServiceCollection services)
        {
            services.AddScoped<IConfigurationRepository, ConfigurationRepository>();

            services.AddSingleton<IValidator<ConfigurationDictionaryGetInput>, ConfigurationDictionaryGetValidation>();
            services.AddSingleton<IValidator<ConfigurationDictionaryInput>, ConfigurationDictionaryValuesValidation>();
            services.AddSingleton<IValidator<ConfigurationDictionaryValueGetInput>, ConfigurationDictionaryValueGetValidation>();
            services.AddSingleton<IValidator<ConfigurationModuleInput>, ConfigurationModuleGetValidation>();

            services.AddScoped<UseCaseHandlerBase<ConfigurationDictionaryGetInput, List<ConfigurationDictionaryGetOutput>>, ConfigurationGetDictionaryUseCase>();
            services.AddScoped<UseCaseHandlerBase<ConfigurationDictionaryValueGetInput, List<ConfigurationDictionaryValueGetOutput>>, ConfigurationGetDictionaryValueUseCase>();
            services.AddScoped<UseCaseHandlerBase<ConfigurationDictionaryInput, ConfigurationDictionaryOutput>, ConfigurationDictionaryValuesUseCase>();

            services.AddScoped<UseCaseHandlerBase<ConfigurationModuleInput, List<ConfigurationModuleOutput>>, ConfigurationGetModuleUseCase>();

            return services;
        }
    }
}
