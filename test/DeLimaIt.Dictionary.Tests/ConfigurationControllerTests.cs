using DeLimaIt.Dictionary.Api.Controllers;
using DeLimaIt.Dictionary.Api.Transport.V1;
using DeLimaIt.Dictionary.Application.Features.Configuration.Models;
using DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities;
using DeLimaIt.Dictionary.Application.Features.Configuration.Validations;
using DelimaIt.Core.UseCases;
using DelimaIt.Core.UseCases.Outputs;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace DeLimaIt.Dictionary.Tests;

public sealed class ConfigurationControllerTests
{
    [Fact]
    public async Task Read_actions_return_success_results()
    {
        var dictionaries = new List<ConfigurationDictionaryGetOutput> { new(1, "colors") };
        var values = new List<ConfigurationDictionaryValueGetOutput> { new("red", "Vermelho") };
        var modules = new List<ConfigurationModuleOutput> { new() { ModuleId = 3, ModuleName = "billing" } };
        var controller = CreateController(dictionaries, values, new ConfigurationDictionaryOutput(), modules);

        var dictionariesResult = Assert.IsType<OkObjectResult>(await controller.GetDictionaries(1, CancellationToken.None));
        var valuesResult = Assert.IsType<OkObjectResult>(await controller.GetDictionaryValues(1, CancellationToken.None));
        var modulesResult = Assert.IsType<OkObjectResult>(await controller.GetModules(1, CancellationToken.None));

        Assert.Same(dictionaries, dictionariesResult.Value);
        Assert.Same(values, valuesResult.Value);
        Assert.Same(modules, modulesResult.Value);
    }

    [Fact]
    public async Task Read_actions_return_bad_request_when_validation_fails()
    {
        var controller = CreateController(new(), new(), new(), new());

        Assert.IsType<BadRequestObjectResult>(await controller.GetDictionaries(-1, CancellationToken.None));
        Assert.IsType<BadRequestObjectResult>(await controller.GetDictionaryValues(-1, CancellationToken.None));
        Assert.IsType<BadRequestObjectResult>(await controller.GetModules(-1, CancellationToken.None));
    }

    [Fact]
    public async Task Mutation_actions_return_success_results()
    {
        var response = new ConfigurationDictionaryOutput { RecordsAffect = 1, Message = "ok" };
        var controller = CreateController(new(), new(), response, new());

        var deleted = Assert.IsType<OkObjectResult>(await controller.DeleteDictionaryValues(1, "key", CancellationToken.None));
        var updated = Assert.IsType<OkObjectResult>(await controller.UpdateDictionaryValues(1, new DictionaryValueRequest("key", "new"), CancellationToken.None));
        var inserted = Assert.IsType<OkObjectResult>(await controller.InsertDictionaryValues(1, new DictionaryValueRequest("key", "value"), CancellationToken.None));

        Assert.Same(response, deleted.Value);
        Assert.Same(response, updated.Value);
        Assert.Same(response, inserted.Value);
    }

    [Fact]
    public async Task Mutation_actions_return_bad_request_when_validation_fails()
    {
        var controller = CreateController(new(), new(), new(), new());

        Assert.IsType<BadRequestObjectResult>(await controller.DeleteDictionaryValues(-1, "key", CancellationToken.None));
        Assert.IsType<BadRequestObjectResult>(await controller.UpdateDictionaryValues(-1, new DictionaryValueRequest("key", "value"), CancellationToken.None));
        Assert.IsType<BadRequestObjectResult>(await controller.InsertDictionaryValues(-1, new DictionaryValueRequest("key", "value"), CancellationToken.None));
    }

    [Fact]
    public void Transport_models_support_constructor_and_property_round_trips()
    {
        var dictionary = new DictionaryResponse(2, "colors");
        var valueRequest = new DictionaryValueRequest("red", "Vermelho");
        var valueResponse = new DictionaryValueResponse(3, "blue");
        var module = new ModuleResponse(4, "billing");

        Assert.Equal(2, dictionary.Id);
        Assert.Equal("colors", dictionary.Name);
        Assert.Equal("red", valueRequest.Key);
        Assert.Equal("Vermelho", valueRequest.Value);
        Assert.Equal(3, valueResponse.Key);
        Assert.Equal("blue", valueResponse.Value);
        Assert.Equal(4, module.Id);
        Assert.Equal("billing", module.Name);

        var emptyDictionary = new DictionaryResponse { Id = 5, Name = "sizes" };
        var emptyValueRequest = new DictionaryValueRequest { Key = "s", Value = "small" };
        var emptyValueResponse = new DictionaryValueResponse { Key = 6, Value = "large" };
        var emptyModule = new ModuleResponse { Id = 7, Name = "inventory" };
        Assert.Equal(5, emptyDictionary.Id);
        Assert.Equal("sizes", emptyDictionary.Name);
        Assert.Equal("s", emptyValueRequest.Key);
        Assert.Equal("small", emptyValueRequest.Value);
        Assert.Equal(6, emptyValueResponse.Key);
        Assert.Equal("large", emptyValueResponse.Value);
        Assert.Equal(7, emptyModule.Id);
        Assert.Equal("inventory", emptyModule.Name);
    }

    private static ConfigurationController CreateController(
        List<ConfigurationDictionaryGetOutput> dictionaries,
        List<ConfigurationDictionaryValueGetOutput> values,
        ConfigurationDictionaryOutput mutation,
        List<ConfigurationModuleOutput> modules)
    {
        return new ConfigurationController(
            new FixedUseCase<ConfigurationDictionaryGetInput, List<ConfigurationDictionaryGetOutput>>(
                new ConfigurationDictionaryGetValidation(), _ => dictionaries),
            new FixedUseCase<ConfigurationDictionaryValueGetInput, List<ConfigurationDictionaryValueGetOutput>>(
                new ConfigurationDictionaryValueGetValidation(), _ => values),
            new FixedUseCase<ConfigurationDictionaryInput, ConfigurationDictionaryOutput>(
                new ConfigurationDictionaryValuesValidation(), _ => mutation),
            new FixedUseCase<ConfigurationModuleInput, List<ConfigurationModuleOutput>>(
                new ConfigurationModuleGetValidation(), _ => modules));
    }

    private sealed class FixedUseCase<TRequest, TResponse> : UseCaseHandlerBase<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        private readonly Func<TRequest, TResponse> _resultFactory;

        public FixedUseCase(IValidator<TRequest> validator, Func<TRequest, TResponse> resultFactory)
            : base(NullLogger<UseCaseHandlerBase<TRequest, TResponse>>.Instance, validator)
        {
            _resultFactory = resultFactory;
        }

        protected override Task<Output<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken)
            => Task.FromResult(new Output<TResponse>(_resultFactory(request)));
    }
}
