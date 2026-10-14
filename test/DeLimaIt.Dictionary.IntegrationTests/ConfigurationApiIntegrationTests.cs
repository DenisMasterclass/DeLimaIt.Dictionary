using System.Net;
using System.Net.Http.Json;
using DeLimaIt.Dictionary.Application.Features.Configuration.Models;

namespace DeLimaIt.Dictionary.IntegrationTests;

[Collection(DatabaseApiCollection.Name)]
public sealed class ConfigurationApiIntegrationTests(DatabaseApiFixture fixture)
{
    [Fact]
    public async Task Read_endpoints_return_records_from_the_seeded_database()
    {
        var modules = await fixture.Client.GetFromJsonAsync<List<ConfigurationModuleOutput>>($"api/Configuration/Modules/{fixture.ModuleId}");
        var dictionaries = await fixture.Client.GetFromJsonAsync<List<ConfigurationDictionaryGetOutput>>($"api/Configuration/Dictionaries/{fixture.ModuleId}");
        var values = await fixture.Client.GetFromJsonAsync<List<ConfigurationDictionaryValueGetOutput>>($"api/Configuration/Dictionaries/Values/{fixture.DictionaryId}");

        Assert.NotNull(modules);
        Assert.Contains(modules, item => item.ModuleId == fixture.ModuleId);
        Assert.NotNull(dictionaries);
        Assert.Contains(dictionaries, item => item.Id == fixture.DictionaryId);
        Assert.NotNull(values);
        Assert.Contains(values, item => item.Key == fixture.SeedKey && item.Value == "seed value");
    }

    [Fact]
    public async Task Insert_update_and_delete_endpoints_persist_database_changes()
    {
        var key = $"integration_{Guid.NewGuid():N}";
        using var insertResponse = await fixture.Client.PostAsJsonAsync(
            $"api/Configuration/Dictionaries/Values/{fixture.DictionaryId}",
            new { Key = key, Value = "inserted" });

        Assert.Equal(HttpStatusCode.OK, insertResponse.StatusCode);
        var insertResult = await insertResponse.Content.ReadFromJsonAsync<ConfigurationDictionaryOutput>();
        Assert.NotNull(insertResult);
        Assert.Equal(1, insertResult.RecordsAffect);

        var updatedKey = $"renamed_{Guid.NewGuid():N}";
        using var updateResponse = await fixture.Client.PutAsJsonAsync(
            $"api/Configuration/Dictionaries/Values/{fixture.DictionaryId}",
            new { OldKey = key, Key = updatedKey, Value = "updated" });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updateResult = await updateResponse.Content.ReadFromJsonAsync<ConfigurationDictionaryOutput>();
        Assert.NotNull(updateResult);
        Assert.Equal(1, updateResult.RecordsAffect);

        var valuesAfterUpdate = await fixture.Client.GetFromJsonAsync<List<ConfigurationDictionaryValueGetOutput>>(
            $"api/Configuration/Dictionaries/Values/{fixture.DictionaryId}");
        Assert.Contains(valuesAfterUpdate!, item => item.Key == updatedKey && item.Value == "updated");

        using var deleteResponse = await fixture.Client.DeleteAsync(
            $"api/Configuration/Dictionaries/Values/{fixture.DictionaryId}/{Uri.EscapeDataString(updatedKey)}");

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        var deleteResult = await deleteResponse.Content.ReadFromJsonAsync<ConfigurationDictionaryOutput>();
        Assert.NotNull(deleteResult);
        Assert.Equal(1, deleteResult.RecordsAffect);

        var valuesAfterDelete = await fixture.Client.GetFromJsonAsync<List<ConfigurationDictionaryValueGetOutput>>(
            $"api/Configuration/Dictionaries/Values/{fixture.DictionaryId}");
        Assert.DoesNotContain(valuesAfterDelete!, item => item.Key == updatedKey);
    }

    [Fact]
    public async Task Modules_and_dictionaries_support_full_crud_and_parent_deletion_cascades()
    {
        var createdModuleId = 0;
        try
        {
        var moduleName = $"integration_module_{Guid.NewGuid():N}";
        using var createModule = await fixture.Client.PostAsJsonAsync("api/Configuration/Modules", new { Name = moduleName });
        Assert.Equal(HttpStatusCode.OK, createModule.StatusCode);
        var createdModule = await createModule.Content.ReadFromJsonAsync<CreatedRecord>();
        Assert.NotNull(createdModule);
        createdModuleId = createdModule.Id;
        using var duplicateModule = await fixture.Client.PostAsJsonAsync("api/Configuration/Modules", new { Name = moduleName });
        Assert.Equal(HttpStatusCode.Conflict, duplicateModule.StatusCode);

        var moduleNameUpdated = $"{moduleName}_updated";
        using var updateModule = await fixture.Client.PutAsJsonAsync($"api/Configuration/Modules/{createdModule.Id}", new { Name = moduleNameUpdated });
        Assert.Equal(HttpStatusCode.OK, updateModule.StatusCode);
        var modules = await fixture.Client.GetFromJsonAsync<List<ConfigurationModuleOutput>>("api/Configuration/Modules/0");
        Assert.Contains(modules!, item => item.ModuleId == createdModule.Id && item.ModuleName == moduleNameUpdated);

        var dictionaryName = $"integration_dictionary_{Guid.NewGuid():N}";
        using var createDictionary = await fixture.Client.PostAsJsonAsync("api/Configuration/Dictionaries", new { ModuleId = createdModule.Id, Name = dictionaryName });
        Assert.Equal(HttpStatusCode.OK, createDictionary.StatusCode);
        var createdDictionary = await createDictionary.Content.ReadFromJsonAsync<CreatedRecord>();
        Assert.NotNull(createdDictionary);
        using var duplicateDictionary = await fixture.Client.PostAsJsonAsync("api/Configuration/Dictionaries", new { ModuleId = createdModule.Id, Name = dictionaryName });
        Assert.Equal(HttpStatusCode.Conflict, duplicateDictionary.StatusCode);
        var listedDictionaries = await fixture.Client.GetFromJsonAsync<List<ConfigurationDictionaryGetOutput>>($"api/Configuration/Dictionaries/{createdModule.Id}");
        Assert.Contains(listedDictionaries!, item => item.Id == createdDictionary.Id && item.Name == dictionaryName);

        var dictionaryNameUpdated = $"{dictionaryName}_updated";
        using var updateDictionary = await fixture.Client.PutAsJsonAsync($"api/Configuration/Dictionaries/{createdDictionary.Id}", new { Name = dictionaryNameUpdated });
        Assert.Equal(HttpStatusCode.OK, updateDictionary.StatusCode);
        listedDictionaries = await fixture.Client.GetFromJsonAsync<List<ConfigurationDictionaryGetOutput>>($"api/Configuration/Dictionaries/{createdModule.Id}");
        Assert.Contains(listedDictionaries!, item => item.Id == createdDictionary.Id && item.Name == dictionaryNameUpdated);

        using var createValue = await fixture.Client.PostAsJsonAsync($"api/Configuration/Dictionaries/Values/{createdDictionary.Id}", new { Key = "cascade", Value = "removed" });
        Assert.Equal(HttpStatusCode.OK, createValue.StatusCode);
        using var duplicateValue = await fixture.Client.PostAsJsonAsync($"api/Configuration/Dictionaries/Values/{createdDictionary.Id}", new { Key = "cascade", Value = "duplicate" });
        Assert.Equal(HttpStatusCode.Conflict, duplicateValue.StatusCode);

        using var deleteDictionary = await fixture.Client.DeleteAsync($"api/Configuration/Dictionaries/{createdDictionary.Id}");
        Assert.Equal(HttpStatusCode.OK, deleteDictionary.StatusCode);
        listedDictionaries = await fixture.Client.GetFromJsonAsync<List<ConfigurationDictionaryGetOutput>>($"api/Configuration/Dictionaries/{createdModule.Id}");
        Assert.DoesNotContain(listedDictionaries!, item => item.Id == createdDictionary.Id);
        var valuesAfterDictionaryDelete = await fixture.Client.GetFromJsonAsync<List<ConfigurationDictionaryValueGetOutput>>($"api/Configuration/Dictionaries/Values/{createdDictionary.Id}");
        Assert.Empty(valuesAfterDictionaryDelete!);

        using var childDictionaryResponse = await fixture.Client.PostAsJsonAsync("api/Configuration/Dictionaries", new { ModuleId = createdModule.Id, Name = $"module_child_{Guid.NewGuid():N}" });
        Assert.Equal(HttpStatusCode.OK, childDictionaryResponse.StatusCode);
        var childDictionary = await childDictionaryResponse.Content.ReadFromJsonAsync<CreatedRecord>();
        Assert.NotNull(childDictionary);
        using var childValueResponse = await fixture.Client.PostAsJsonAsync($"api/Configuration/Dictionaries/Values/{childDictionary.Id}", new { Key = "module-cascade", Value = "removed" });
        Assert.Equal(HttpStatusCode.OK, childValueResponse.StatusCode);

        using var deleteModule = await fixture.Client.DeleteAsync($"api/Configuration/Modules/{createdModule.Id}");
        Assert.Equal(HttpStatusCode.OK, deleteModule.StatusCode);
        modules = await fixture.Client.GetFromJsonAsync<List<ConfigurationModuleOutput>>("api/Configuration/Modules/0");
        Assert.DoesNotContain(modules!, item => item.ModuleId == createdModule.Id);
        var childValuesAfterModuleDelete = await fixture.Client.GetFromJsonAsync<List<ConfigurationDictionaryValueGetOutput>>($"api/Configuration/Dictionaries/Values/{childDictionary.Id}");
        Assert.Empty(childValuesAfterModuleDelete!);
        }
        finally
        {
            if (createdModuleId > 0)
            {
                using var cleanup = await fixture.Client.DeleteAsync($"api/Configuration/Modules/{createdModuleId}");
            }
        }
    }

    [Fact]
    public async Task Endpoints_reject_negative_identifiers()
    {
        using var dictionariesResponse = await fixture.Client.GetAsync("api/Configuration/Dictionaries/-1");
        using var valuesResponse = await fixture.Client.GetAsync("api/Configuration/Dictionaries/Values/-1");
        using var insertResponse = await fixture.Client.PostAsJsonAsync(
            "api/Configuration/Dictionaries/Values/-1",
            new { Key = "key", Value = "value" });

        Assert.Equal(HttpStatusCode.BadRequest, dictionariesResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, valuesResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, insertResponse.StatusCode);
    }

    private sealed record CreatedRecord(int Id, int RecordsAffected);
}
