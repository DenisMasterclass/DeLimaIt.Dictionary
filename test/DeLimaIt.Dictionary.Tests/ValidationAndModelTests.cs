using DeLimaIt.Dictionary.Application.Features.Configuration.Models;
using DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities;
using DeLimaIt.Dictionary.Application.Features.Configuration.Validations;

namespace DeLimaIt.Dictionary.Tests;

public sealed class ValidationAndModelTests
{
    [Theory]
    [InlineData(0, true)]
    [InlineData(9, true)]
    [InlineData(-1, false)]
    public async Task Dictionary_get_validator_checks_module_id(int moduleId, bool expectedValid)
    {
        var result = await new ConfigurationDictionaryGetValidation().ValidateAsync(new ConfigurationDictionaryGetInput(moduleId));
        Assert.Equal(expectedValid, result.IsValid);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(9, true)]
    [InlineData(-1, false)]
    public async Task Dictionary_value_get_validator_checks_dictionary_id(int dictionaryId, bool expectedValid)
    {
        var result = await new ConfigurationDictionaryValueGetValidation().ValidateAsync(new ConfigurationDictionaryValueGetInput(dictionaryId));
        Assert.Equal(expectedValid, result.IsValid);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(9, true)]
    [InlineData(-1, false)]
    public async Task Dictionary_mutation_validator_checks_dictionary_id(int dictionaryId, bool expectedValid)
    {
        var request = new ConfigurationDictionaryInput(OperationType.Update, dictionaryId, "k", "v");
        var result = await new ConfigurationDictionaryValuesValidation().ValidateAsync(request);
        Assert.Equal(expectedValid, result.IsValid);
    }

    [Fact]
    public void Input_constructors_preserve_values_and_correlation_identifiers()
    {
        var correlationId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var dictionaryRequest = new ConfigurationDictionaryInput(OperationType.Delete, 3, "k", "v", correlationId, transactionId);
        var configurationRequest = new ConfigurationGetInput(4, "name", correlationId, transactionId);
        var dictionaryGet = new ConfigurationDictionaryGetInput(5, correlationId, transactionId);
        var valueGet = new ConfigurationDictionaryValueGetInput(6, correlationId, transactionId);
        var moduleRequest = new ConfigurationModuleInput(new ConfigurationModuleEntity(), correlationId, transactionId);

        Assert.Equal(3, dictionaryRequest.DictionaryId);
        Assert.Equal("k", dictionaryRequest.Key);
        Assert.Equal("v", dictionaryRequest.Value);
        Assert.Equal(correlationId, dictionaryRequest.CorrelationId);
        Assert.Equal(transactionId, dictionaryRequest.TransactionId);
        Assert.Equal("name", configurationRequest.DictionaryId);
        Assert.Equal(4, configurationRequest.ModuleId);
        Assert.Equal(5, dictionaryGet.ModuleId);
        Assert.Equal(correlationId, dictionaryGet.CorrelationId);
        Assert.Equal(transactionId, dictionaryGet.TransactionId);
        Assert.Equal(6, valueGet.DictionaryId);
        Assert.Equal(correlationId, valueGet.CorrelationId);
        Assert.Equal(transactionId, valueGet.TransactionId);
        Assert.Equal(correlationId, moduleRequest.CorrelationId);
        Assert.Equal(transactionId, moduleRequest.TransactionId);
    }

    [Fact]
    public void Input_constructors_generate_identifiers_when_empty()
    {
        var dictionaryRequest = new ConfigurationDictionaryInput(OperationType.Insert, 1, "k", "v", Guid.Empty, Guid.Empty);
        var configurationRequest = new ConfigurationGetInput(2, "d", Guid.Empty, Guid.Empty);
        var dictionaryGet = new ConfigurationDictionaryGetInput(3, Guid.Empty, Guid.Empty);
        var valueGet = new ConfigurationDictionaryValueGetInput(4, Guid.Empty, Guid.Empty);
        var module = new ConfigurationModuleEntity { ModuleId = 8, ModuleName = "m" };
        var moduleRequest = new ConfigurationModuleInput(module, Guid.Empty, Guid.Empty);
        var explicitIds = new ConfigurationModuleInput(module, Guid.NewGuid(), Guid.NewGuid());

        Assert.NotEqual(Guid.Empty, dictionaryRequest.CorrelationId);
        Assert.NotEqual(Guid.Empty, dictionaryRequest.TransactionId);
        Assert.NotEqual(Guid.Empty, configurationRequest.CorrelationId);
        Assert.NotEqual(Guid.Empty, configurationRequest.TransactionId);
        Assert.NotEqual(Guid.Empty, dictionaryGet.CorrelationId);
        Assert.NotEqual(Guid.Empty, dictionaryGet.TransactionId);
        Assert.NotEqual(Guid.Empty, valueGet.CorrelationId);
        Assert.NotEqual(Guid.Empty, valueGet.TransactionId);
        Assert.NotEqual(Guid.Empty, moduleRequest.CorrelationId);
        Assert.NotEqual(Guid.Empty, moduleRequest.TransactionId);
        Assert.Same(module, moduleRequest.Entity);
        Assert.NotEqual(Guid.Empty, explicitIds.CorrelationId);
        Assert.NotEqual(Guid.Empty, explicitIds.TransactionId);
    }

    [Fact]
    public void Entity_and_output_constructors_and_properties_round_trip()
    {
        var value = new DictionaryValueEntity(2, "key", "value");
        var configuration = new ConfigurationEntity(1, "dictionary", new List<DictionaryEntity>());
        var output = new ConfigurationDictionaryGetOutput(3, "name") { Id = 4, Name = "renamed" };
        var valueOutput = new ConfigurationDictionaryValueGetOutput("a", "b") { Key = "c", Value = "d" };
        var moduleOutput = new ConfigurationModuleOutput { ModuleId = 8, ModuleName = "module" };
        var dictionary = new DictionaryEntity(10, "catalog");
        var dictionaryFilter = new DictionaryFilter(11);
        var moduleFilter = new ConfigurationDictionaryFilter(12);
        var oldFilter = new ConfigurationFilterEntity(13);

        Assert.Equal(2, value.DictionaryId);
        Assert.Equal("key", value.Key);
        Assert.Equal("value", value.Value);
        Assert.Equal(1, configuration.ModuleId);
        Assert.Equal("dictionary", configuration.DictionaryId);
        Assert.Equal(4, output.Id);
        Assert.Equal("renamed", output.Name);
        Assert.Equal("c", valueOutput.Key);
        Assert.Equal("d", valueOutput.Value);
        Assert.Equal(8, moduleOutput.ModuleId);
        Assert.Equal(10, dictionary.Id);
        Assert.Equal(11, dictionaryFilter.Id);
        Assert.Equal(12, moduleFilter.ModuleId);
        Assert.Equal(13, oldFilter.ModuleId);

        var emptyValue = new DictionaryValueEntity();
        emptyValue.Key = "x";
        emptyValue.Value = "y";
        var emptyOutput = new ConfigurationDictionaryOutput { RecordsAffect = 2, Message = "done" };
        var emptyModule = new ConfigurationModuleEntity { ModuleId = 14, ModuleName = "other" };
        Assert.Equal("x", emptyValue.Key);
        Assert.Equal("y", emptyValue.Value);
        Assert.Equal(2, emptyOutput.RecordsAffect);
        Assert.Equal("done", emptyOutput.Message);
        Assert.Equal(14, emptyModule.ModuleId);
        Assert.Equal("other", emptyModule.ModuleName);
    }
}
