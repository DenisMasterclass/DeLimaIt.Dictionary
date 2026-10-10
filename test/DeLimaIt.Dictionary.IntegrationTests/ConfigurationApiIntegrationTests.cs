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

        using var updateResponse = await fixture.Client.PutAsJsonAsync(
            $"api/Configuration/Dictionaries/Values/{fixture.DictionaryId}",
            new { Key = key, Value = "updated" });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updateResult = await updateResponse.Content.ReadFromJsonAsync<ConfigurationDictionaryOutput>();
        Assert.NotNull(updateResult);
        Assert.Equal(1, updateResult.RecordsAffect);

        var valuesAfterUpdate = await fixture.Client.GetFromJsonAsync<List<ConfigurationDictionaryValueGetOutput>>(
            $"api/Configuration/Dictionaries/Values/{fixture.DictionaryId}");
        Assert.Contains(valuesAfterUpdate!, item => item.Key == key && item.Value == "updated");

        using var deleteResponse = await fixture.Client.DeleteAsync(
            $"api/Configuration/Dictionaries/Values/{fixture.DictionaryId}/{Uri.EscapeDataString(key)}");

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        var deleteResult = await deleteResponse.Content.ReadFromJsonAsync<ConfigurationDictionaryOutput>();
        Assert.NotNull(deleteResult);
        Assert.Equal(1, deleteResult.RecordsAffect);

        var valuesAfterDelete = await fixture.Client.GetFromJsonAsync<List<ConfigurationDictionaryValueGetOutput>>(
            $"api/Configuration/Dictionaries/Values/{fixture.DictionaryId}");
        Assert.DoesNotContain(valuesAfterDelete!, item => item.Key == key);
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
}
