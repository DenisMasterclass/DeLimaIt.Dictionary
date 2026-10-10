using DeLimaIt.Dictionary.Application.Features.Configuration;
using DeLimaIt.Dictionary.Application.Features.Configuration.Models;
using DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities;
using DeLimaIt.Dictionary.Application.Features.Configuration.UseCase;
using DeLimaIt.Dictionary.Application.Features.Configuration.Validations;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace DeLimaIt.Dictionary.Tests;

public sealed class ConfigurationUseCaseTests
{
    [Fact]
    public async Task GetDictionaries_maps_repository_rows_and_module_filter()
    {
        var repository = new Mock<IConfigurationRepository>();
        repository.Setup(x => x.GetDictionariesAsync(It.IsAny<ConfigurationDictionaryFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new DictionaryEntity(3, "colors"), new DictionaryEntity(4, "sizes") });
        var handler = new ConfigurationGetDictionaryUseCase(repository.Object, new ConfigurationDictionaryGetValidation(), NullLogger<ConfigurationGetDictionaryUseCase>.Instance);

        var result = await handler.ExecuteAsync(new ConfigurationDictionaryGetInput(12), CancellationToken.None);

        Assert.True(result.IsValid);
        Assert.Collection(result.GetResult(),
            item => { Assert.Equal(3, item.Id); Assert.Equal("colors", item.Name); },
            item => { Assert.Equal(4, item.Id); Assert.Equal("sizes", item.Name); });
        repository.Verify(x => x.GetDictionariesAsync(It.Is<ConfigurationDictionaryFilter>(f => f.ModuleId == 12), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetDictionaries_returns_validation_errors_without_calling_repository()
    {
        var repository = new Mock<IConfigurationRepository>();
        var handler = new ConfigurationGetDictionaryUseCase(repository.Object, new ConfigurationDictionaryGetValidation(), NullLogger<ConfigurationGetDictionaryUseCase>.Instance);

        var result = await handler.ExecuteAsync(new ConfigurationDictionaryGetInput(-1), CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(result.ErrorMessages, message => message.Contains("greater than 0", StringComparison.Ordinal));
        repository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetDictionaryValues_maps_repository_rows_and_dictionary_filter()
    {
        var repository = new Mock<IConfigurationRepository>();
        repository.Setup(x => x.GetDictionariesValuesAsync(It.IsAny<DictionaryFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DictionaryValueEntity> { new() { DictionaryId = 7, Key = "red", Value = "Vermelho" } });
        var handler = new ConfigurationGetDictionaryValueUseCase(repository.Object, new ConfigurationDictionaryValueGetValidation(), NullLogger<ConfigurationGetDictionaryValueUseCase>.Instance);

        var result = await handler.ExecuteAsync(new ConfigurationDictionaryValueGetInput(7), CancellationToken.None);

        Assert.True(result.IsValid);
        Assert.Equal(new ConfigurationDictionaryValueGetOutput("red", "Vermelho").Key, result.GetResult()[0].Key);
        Assert.Equal("Vermelho", result.GetResult()[0].Value);
        repository.Verify(x => x.GetDictionariesValuesAsync(It.Is<DictionaryFilter>(f => f.Id == 7), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetDictionaryValues_rejects_negative_id_without_calling_repository()
    {
        var repository = new Mock<IConfigurationRepository>();
        var handler = new ConfigurationGetDictionaryValueUseCase(repository.Object, new ConfigurationDictionaryValueGetValidation(), NullLogger<ConfigurationGetDictionaryValueUseCase>.Instance);

        var result = await handler.ExecuteAsync(new ConfigurationDictionaryValueGetInput(-2), CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Single(result.ErrorMessages);
        repository.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(OperationType.Insert)]
    [InlineData(OperationType.Update)]
    [InlineData(OperationType.Delete)]
    public async Task DictionaryValue_mutations_call_matching_repository_method(OperationType operation)
    {
        var repository = new Mock<IConfigurationRepository>();
        repository.Setup(x => x.InsertDictionaryValue(It.IsAny<DictionaryValueEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);
        repository.Setup(x => x.UpdateDictionaryValue(It.IsAny<DictionaryValueEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);
        repository.Setup(x => x.DeleteDictionaryValue(It.IsAny<DictionaryValueEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var handler = new ConfigurationDictionaryValuesUseCase(repository.Object, new ConfigurationDictionaryValuesValidation(), NullLogger<ConfigurationDictionaryValuesUseCase>.Instance);
        using var cancellation = new CancellationTokenSource();

        var result = await handler.ExecuteAsync(new ConfigurationDictionaryInput(operation, 5, "key", "value"), cancellation.Token);

        Assert.True(result.IsValid);
        Assert.Equal(1, result.GetResult().RecordsAffect);
        Assert.Contains(operation.ToString(), result.GetResult().Message, StringComparison.Ordinal);
        var expectedMethod = operation switch
        {
            OperationType.Insert => nameof(IConfigurationRepository.InsertDictionaryValue),
            OperationType.Update => nameof(IConfigurationRepository.UpdateDictionaryValue),
            _ => nameof(IConfigurationRepository.DeleteDictionaryValue)
        };
        repository.Verify(x => x.InsertDictionaryValue(It.Is<DictionaryValueEntity>(v => v.DictionaryId == 5 && v.Key == "key" && v.Value == "value"), cancellation.Token), operation == OperationType.Insert ? Times.Once() : Times.Never());
        repository.Verify(x => x.UpdateDictionaryValue(It.Is<DictionaryValueEntity>(v => v.DictionaryId == 5 && v.Key == "key" && v.Value == "value"), cancellation.Token), operation == OperationType.Update ? Times.Once() : Times.Never());
        repository.Verify(x => x.DeleteDictionaryValue(It.Is<DictionaryValueEntity>(v => v.DictionaryId == 5 && v.Key == "key" && v.Value == "value"), cancellation.Token), operation == OperationType.Delete ? Times.Once() : Times.Never());
        Assert.Contains(repository.Invocations, invocation => invocation.Method.Name == expectedMethod);
    }

    [Fact]
    public async Task DictionaryValue_mutation_rejects_negative_id()
    {
        var repository = new Mock<IConfigurationRepository>();
        var handler = new ConfigurationDictionaryValuesUseCase(repository.Object, new ConfigurationDictionaryValuesValidation(), NullLogger<ConfigurationDictionaryValuesUseCase>.Instance);

        var result = await handler.ExecuteAsync(new ConfigurationDictionaryInput(OperationType.Insert, -1, "key", "value"), CancellationToken.None);

        Assert.False(result.IsValid);
        repository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DictionaryValue_mutation_rejects_unknown_operation()
    {
        var repository = new Mock<IConfigurationRepository>();
        var handler = new ConfigurationDictionaryValuesUseCase(repository.Object, new ConfigurationDictionaryValuesValidation(), NullLogger<ConfigurationDictionaryValuesUseCase>.Instance);

        var result = await handler.ExecuteAsync(new ConfigurationDictionaryInput((OperationType)99, 1, "key", "value"), CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains("Unsupported dictionary operation.", result.ErrorMessages);
        repository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetModules_returns_repository_result()
    {
        var modules = new List<ConfigurationModuleOutput> { new() { ModuleId = 2, ModuleName = "billing" } };
        var repository = new Mock<IConfigurationRepository>();
        repository.Setup(x => x.GetModulesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(modules);
        var handler = new ConfigurationGetModuleUseCase(repository.Object, NullLogger<ConfigurationGetModuleUseCase>.Instance);

        var result = await handler.ExecuteAsync(new ConfigurationModuleInput(new ConfigurationModuleEntity()), CancellationToken.None);

        Assert.True(result.IsValid);
        Assert.Same(modules, result.GetResult());
        repository.Verify(x => x.GetModulesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(4, true)]
    [InlineData(-1, false)]
    public async Task Module_validator_checks_module_id(int moduleId, bool expectedValid)
    {
        var validator = new ConfigurationModuleGetValidation();
        var result = await validator.ValidateAsync(new ConfigurationModuleInput(new ConfigurationModuleEntity { ModuleId = moduleId }));

        Assert.Equal(expectedValid, result.IsValid);
    }
}
