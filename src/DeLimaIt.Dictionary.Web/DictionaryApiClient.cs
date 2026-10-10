using System.Net.Http.Json;

namespace DeLimaIt.Dictionary.Web;

public sealed class DictionaryApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<ModuleItem>> GetModulesAsync(CancellationToken cancellationToken = default) =>
        await httpClient.GetFromJsonAsync<List<ModuleItem>>("api/Configuration/Modules/0", cancellationToken) ?? [];

    public async Task<IReadOnlyList<DictionaryItem>> GetDictionariesAsync(int moduleId, CancellationToken cancellationToken = default) =>
        await httpClient.GetFromJsonAsync<List<DictionaryItem>>($"api/Configuration/Dictionaries/{moduleId}", cancellationToken) ?? [];

    public async Task<IReadOnlyList<DictionaryValue>> GetValuesAsync(int dictionaryId, CancellationToken cancellationToken = default) =>
        await httpClient.GetFromJsonAsync<List<DictionaryValue>>($"api/Configuration/Dictionaries/Values/{dictionaryId}", cancellationToken) ?? [];

    public Task<HttpResponseMessage> InsertValueAsync(int dictionaryId, DictionaryValue value, CancellationToken cancellationToken = default) =>
        httpClient.PostAsJsonAsync($"api/Configuration/Dictionaries/Values/{dictionaryId}", value, cancellationToken);

    public Task<HttpResponseMessage> UpdateValueAsync(int dictionaryId, DictionaryValue value, CancellationToken cancellationToken = default) =>
        httpClient.PutAsJsonAsync($"api/Configuration/Dictionaries/Values/{dictionaryId}", value, cancellationToken);

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
