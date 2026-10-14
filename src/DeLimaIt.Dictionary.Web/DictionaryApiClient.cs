using System.Net.Http.Json;

namespace DeLimaIt.Dictionary.Web;

public sealed class DictionaryApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<ModuleItem>> GetModulesAsync(CancellationToken cancellationToken = default) =>
        await httpClient.GetFromJsonAsync<List<ModuleItem>>("api/Configuration/Modules/0", cancellationToken) ?? [];

    public Task<HttpResponseMessage> CreateModuleAsync(string name, CancellationToken cancellationToken = default) =>
        httpClient.PostAsJsonAsync("api/Configuration/Modules", new { Name = name }, cancellationToken);

    public Task<HttpResponseMessage> UpdateModuleAsync(int moduleId, string name, CancellationToken cancellationToken = default) =>
        httpClient.PutAsJsonAsync($"api/Configuration/Modules/{moduleId}", new { Name = name }, cancellationToken);

    public Task<HttpResponseMessage> DeleteModuleAsync(int moduleId, CancellationToken cancellationToken = default) =>
        httpClient.DeleteAsync($"api/Configuration/Modules/{moduleId}", cancellationToken);

    public async Task<IReadOnlyList<DictionaryItem>> GetDictionariesAsync(int moduleId, CancellationToken cancellationToken = default) =>
        await httpClient.GetFromJsonAsync<List<DictionaryItem>>($"api/Configuration/Dictionaries/{moduleId}", cancellationToken) ?? [];

    public Task<HttpResponseMessage> CreateDictionaryAsync(int moduleId, string name, CancellationToken cancellationToken = default) =>
        httpClient.PostAsJsonAsync("api/Configuration/Dictionaries", new { ModuleId = moduleId, Name = name }, cancellationToken);

    public Task<HttpResponseMessage> UpdateDictionaryAsync(int dictionaryId, string name, CancellationToken cancellationToken = default) =>
        httpClient.PutAsJsonAsync($"api/Configuration/Dictionaries/{dictionaryId}", new { Name = name }, cancellationToken);

    public Task<HttpResponseMessage> DeleteDictionaryAsync(int dictionaryId, CancellationToken cancellationToken = default) =>
        httpClient.DeleteAsync($"api/Configuration/Dictionaries/{dictionaryId}", cancellationToken);

    public async Task<IReadOnlyList<DictionaryValue>> GetValuesAsync(int dictionaryId, CancellationToken cancellationToken = default) =>
        await httpClient.GetFromJsonAsync<List<DictionaryValue>>($"api/Configuration/Dictionaries/Values/{dictionaryId}", cancellationToken) ?? [];

    public Task<HttpResponseMessage> InsertValueAsync(int dictionaryId, DictionaryValue value, CancellationToken cancellationToken = default) =>
        httpClient.PostAsJsonAsync($"api/Configuration/Dictionaries/Values/{dictionaryId}", value, cancellationToken);

    public Task<HttpResponseMessage> UpdateValueAsync(int dictionaryId, DictionaryValue value, string? oldKey = null, CancellationToken cancellationToken = default) =>
        httpClient.PutAsJsonAsync($"api/Configuration/Dictionaries/Values/{dictionaryId}", new { OldKey = oldKey ?? value.Key, value.Key, value.Value }, cancellationToken);

    public Task<HttpResponseMessage> DeleteValueAsync(int dictionaryId, string key, CancellationToken cancellationToken = default) =>
        httpClient.DeleteAsync($"api/Configuration/Dictionaries/Values/{dictionaryId}/{Uri.EscapeDataString(key)}", cancellationToken);

    public static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        if (response.IsSuccessStatusCode)
            return;

        var details = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new InvalidOperationException(string.IsNullOrWhiteSpace(details)
            ? $"A API respondeu com {(int)response.StatusCode} ({response.StatusCode})."
            : $"A API respondeu com {(int)response.StatusCode}: {details}");
    }
}

public sealed record ModuleItem(int ModuleId, string? ModuleName);
public sealed record DictionaryItem(int Id, string Name);
public sealed record DictionaryValue(string Key, string Value);
