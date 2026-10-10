using DeLimaIt.Dictionary.Application.Features.Configuration;
using DeLimaIt.Dictionary.Application.Features.Configuration.DependencyInjection;
using DeLimaIt.Dictionary.Application.Features.Configuration.Models;
using DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities;
using DeLimaIt.Dictionary.Application.Features.Configuration.Validations;
using DeLimaIt.Dictionary.Application.Shared.Repository;
using DeLimaIt.Dictionary.Shared;
using DelimaIt.Core.UseCases;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace DeLimaIt.Dictionary.Tests;

public sealed class RegistrationAndMiscTests
{
    [Fact]
    public void Application_registration_adds_repository_validators_and_handlers()
    {
        var services = new ServiceCollection();

        services.AddConfigurationDictionaries();

        Assert.Contains(services, item => item.ServiceType == typeof(IConfigurationRepository));
        Assert.Contains(services, item => item.ServiceType == typeof(IValidator<ConfigurationDictionaryGetInput>));
        Assert.Contains(services, item => item.ServiceType == typeof(IValidator<ConfigurationDictionaryInput>));
        Assert.Contains(services, item => item.ServiceType == typeof(IValidator<ConfigurationDictionaryValueGetInput>));
        Assert.Contains(services, item => item.ServiceType == typeof(IValidator<ConfigurationModuleInput>));
        Assert.Contains(services, item => item.ServiceType == typeof(UseCaseHandlerBase<ConfigurationDictionaryGetInput, List<ConfigurationDictionaryGetOutput>>));
        Assert.Contains(services, item => item.ServiceType == typeof(UseCaseHandlerBase<ConfigurationDictionaryValueGetInput, List<ConfigurationDictionaryValueGetOutput>>));
        Assert.Contains(services, item => item.ServiceType == typeof(UseCaseHandlerBase<ConfigurationDictionaryInput, ConfigurationDictionaryOutput>));
        Assert.Contains(services, item => item.ServiceType == typeof(UseCaseHandlerBase<ConfigurationModuleInput, List<ConfigurationModuleOutput>>));
    }

    [Fact]
    public void Sql_server_registration_adds_context_factory_and_health_checks_without_connecting()
    {
        var services = new ServiceCollection();
        services.AddSqlServerDictionary("Server=localhost;Database=unused;Trusted_Connection=True;", "00:00:30");

        Assert.Contains(services, item => item.ServiceType == typeof(DeLimaIt.Dictionary.Application.Shared.DataContext));
        Assert.Contains(services, item => item.ServiceType.FullName!.Contains("HealthCheck", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("00:00:05", 5)]
    [InlineData("invalid", 15)]
    public void Sql_server_health_check_uses_parsed_timeout_or_default(string configuredTimeout, int expectedSeconds)
    {
        var services = new ServiceCollection();
        services.AddSqlHealthCheck("Server=localhost;Database=unused;", configuredTimeout);
        using var provider = services.BuildServiceProvider();

        var registration = provider.GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value.Registrations.Single();

        Assert.Equal(TimeSpan.FromSeconds(expectedSeconds), registration.Timeout);
    }

    [Fact]
    public void Error_value_has_value_equality_and_hash_code()
    {
        var first = new Error("E1", "failure");
        var same = new Error("E1", "failure");
        var different = new Error("E2", "failure");

        Assert.Equal("E1", first.ErrorCode);
        Assert.Equal("failure", first.ErrorMessage);
        Assert.True(first.Equals(same));
        Assert.True(first.Equals((object)same));
        Assert.False(first.Equals(different));
        Assert.False(first.Equals("not an Error"));
        Assert.Equal(first.GetHashCode(), same.GetHashCode());
    }

    [Fact]
    public void Remaining_model_properties_and_default_constructors_round_trip()
    {
        var configurationGetOutput = new ConfigurationGetOutput { ModuleId = 2, DictionaryId = "dict", Dictionaries = new List<DictionaryGetOutput>() };
        var dictionaryGetOutput = new DictionaryGetOutput { Key = "key", Value = "value" };
        var moduleInput = new ConfigurationModuleInput();
        var module = new ConfigurationModuleEntity();
        moduleInput.Entity = module;
        var dictionaryFilter = new ConfigurationDictionaryFilter();
        dictionaryFilter.ModuleId = 3;
        var simpleDictionaryFilter = new DictionaryFilter();
        simpleDictionaryFilter.Id = 7;
        var legacyFilter = new ConfigurationFilterEntity();
        legacyFilter.ModuleId = 4;
        var dictionaryValue = new DictionaryValueEntity { DictionaryId = 5, Key = "x", Value = "y" };
        var config = new ConfigurationEntity { ModuleId = 6, DictionaryId = "d", DictionaryKey = "k", DictionaryValue = "v" };

        Assert.Equal(2, configurationGetOutput.ModuleId);
        Assert.Equal("dict", configurationGetOutput.DictionaryId);
        Assert.Empty(configurationGetOutput.Dictionaries);
        Assert.Equal("key", dictionaryGetOutput.Key);
        Assert.Equal("value", dictionaryGetOutput.Value);
        Assert.Same(module, moduleInput.Entity);
        Assert.Equal(3, dictionaryFilter.ModuleId);
        Assert.Equal(7, simpleDictionaryFilter.Id);
        Assert.Equal(4, legacyFilter.ModuleId);
        Assert.Equal(5, dictionaryValue.DictionaryId);
        Assert.Equal(6, config.ModuleId);
        Assert.Equal("d", config.DictionaryId);
        Assert.Equal("k", config.DictionaryKey);
        Assert.Equal("v", config.DictionaryValue);
    }
}
