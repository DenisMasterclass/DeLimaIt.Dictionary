using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DeLimaIt.Dictionary.IntegrationTests;

[CollectionDefinition("Dictionary API and SQL Server", DisableParallelization = true)]
public sealed class DatabaseApiCollection : ICollectionFixture<DatabaseApiFixture>
{
    public const string Name = "Dictionary API and SQL Server";
}

public sealed class DatabaseApiFixture : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly SqlConnection _connection;
    private readonly string _moduleName = $"integration_{Guid.NewGuid():N}";
    private readonly string _dictionaryName = $"integration_{Guid.NewGuid():N}";

    public HttpClient Client { get; }
    public int ModuleId { get; private set; }
    public int DictionaryId { get; private set; }
    public string SeedKey { get; } = "fixture_seed";

    public DatabaseApiFixture()
    {
        var repositoryRoot = FindRepositoryRoot();
        var apiSettings = Path.Combine(repositoryRoot, "src", "DeLimaIt.Dictionary.Api", "appsettings.Development.json");
        var configuration = new ConfigurationBuilder().AddJsonFile(apiSettings, optional: false).Build();
        var connectionString = configuration["SQL_DELIMAIT_DICTIONARY_CONN_STRING"]
            ?? throw new InvalidOperationException("The development SQL Server connection string is missing.");
        _connection = new SqlConnection(connectionString);

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => builder.UseEnvironment("Development"));
        Client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();
        await using (var command = _connection.CreateCommand())
        {
            command.CommandText = "INSERT INTO dbo.CONFIGURACAO_MODULO (NOM_MODULO) OUTPUT INSERTED.COD_MODULO VALUES (@name);";
            command.Parameters.AddWithValue("@name", _moduleName);
            ModuleId = Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        await using (var command = _connection.CreateCommand())
        {
            command.CommandText = "INSERT INTO dbo.CONFIGURACAO_PARAMETRO (COD_MODULO, NOM_PARAMETRO) OUTPUT INSERTED.COD_PARAMETRO VALUES (@moduleId, @name);";
            command.Parameters.AddWithValue("@moduleId", ModuleId);
            command.Parameters.AddWithValue("@name", _dictionaryName);
            DictionaryId = Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        await using (var command = _connection.CreateCommand())
        {
            command.CommandText = "INSERT INTO dbo.CONFIGURACAO_CHAVE (COD_PARAMETRO, DSC_CHAVE, DSC_VALOR) VALUES (@dictionaryId, @key, @value);";
            command.Parameters.AddWithValue("@dictionaryId", DictionaryId);
            command.Parameters.AddWithValue("@key", SeedKey);
            command.Parameters.AddWithValue("@value", "seed value");
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task DisposeAsync()
    {
        try
        {
            if (_connection.State == System.Data.ConnectionState.Open && ModuleId > 0)
            {
                await using var command = _connection.CreateCommand();
                command.CommandText = @"
DELETE FROM dbo.CONFIGURACAO_CHAVE WHERE COD_PARAMETRO = @dictionaryId;
DELETE FROM dbo.CONFIGURACAO_PARAMETRO WHERE COD_PARAMETRO = @dictionaryId;
DELETE FROM dbo.CONFIGURACAO_MODULO WHERE COD_MODULO = @moduleId;";
                command.Parameters.AddWithValue("@dictionaryId", DictionaryId);
                command.Parameters.AddWithValue("@moduleId", ModuleId);
                await command.ExecuteNonQueryAsync();
            }
        }
        finally
        {
            await _connection.DisposeAsync();
            Client.Dispose();
            await _factory.DisposeAsync();
        }
    }

    private static string FindRepositoryRoot()
    {
        for (var directory = new DirectoryInfo(Directory.GetCurrentDirectory()); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "DeLimaIt.Dictionary.sln")))
            {
                return directory.FullName;
            }
        }

        throw new DirectoryNotFoundException("Could not locate the repository root from the current directory.");
    }
}
