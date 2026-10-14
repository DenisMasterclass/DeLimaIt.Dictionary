using DeLimaIt.Dictionary.Api.Transport.V1;
using DeLimaIt.Dictionary.Application.Features.Configuration;
using DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DeLimaIt.Dictionary.Api.Controllers;

[Route("api/Configuration")]
[ApiController]
public sealed class ConfigurationAdminController(IConfigurationRepository repository) : ControllerBase
{
    [HttpPost("Modules")]
    public async Task<IActionResult> CreateModule([FromBody] ConfigurationModuleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var id = await repository.CreateModuleAsync(new ConfigurationModuleEntity { ModuleName = request.Name.Trim() }, cancellationToken);
            return Ok(new { id, recordsAffected = 1 });
        }
        catch (SqlException ex) when (IsUniqueViolation(ex)) { return Conflict("Já existe um módulo com esse nome."); }
    }

    [HttpPut("Modules/{moduleId:int}")]
    public async Task<IActionResult> UpdateModule(int moduleId, [FromBody] ConfigurationModuleRequest request, CancellationToken cancellationToken)
    {
        if (moduleId <= 0) return BadRequest("O identificador do módulo deve ser positivo.");
        try
        {
            var affected = await repository.UpdateModule(new ConfigurationModuleEntity { ModuleId = moduleId, ModuleName = request.Name.Trim() }, cancellationToken);
            return affected == 0 ? NotFound() : Ok(new { recordsAffected = affected });
        }
        catch (SqlException ex) when (IsUniqueViolation(ex)) { return Conflict("Já existe um módulo com esse nome."); }
    }

    [HttpDelete("Modules/{moduleId:int}")]
    public async Task<IActionResult> DeleteModule(int moduleId, CancellationToken cancellationToken)
    {
        if (moduleId <= 0) return BadRequest("O identificador do módulo deve ser positivo.");
        var affected = await repository.DeleteModule(new ConfigurationModuleEntity { ModuleId = moduleId }, cancellationToken);
        return affected == 0 ? NotFound() : Ok(new { recordsAffected = affected });
    }

    [HttpPost("Dictionaries")]
    public async Task<IActionResult> CreateDictionary([FromBody] ConfigurationParameterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var id = await repository.CreateDictionaryAsync(new ConfigurationParameterEntity { ModuleId = request.ModuleId, Name = request.Name.Trim() }, cancellationToken);
            return Ok(new { id, recordsAffected = 1 });
        }
        catch (SqlException ex) when (IsUniqueViolation(ex)) { return Conflict("Já existe um dicionário com esse nome neste módulo."); }
        catch (SqlException ex) when (IsForeignKeyViolation(ex)) { return BadRequest("O módulo informado não existe."); }
    }

    [HttpPut("Dictionaries/{dictionaryId:int}")]
    public async Task<IActionResult> UpdateDictionary(int dictionaryId, [FromBody] ConfigurationParameterUpdateRequest request, CancellationToken cancellationToken)
    {
        if (dictionaryId <= 0) return BadRequest("O identificador do dicionário deve ser positivo.");
        try
        {
            var affected = await repository.UpdateDictionaryAsync(dictionaryId, request.Name.Trim(), cancellationToken);
            return affected == 0 ? NotFound() : Ok(new { recordsAffected = affected });
        }
        catch (SqlException ex) when (IsUniqueViolation(ex)) { return Conflict("Já existe um dicionário com esse nome neste módulo."); }
    }

    [HttpDelete("Dictionaries/{dictionaryId:int}")]
    public async Task<IActionResult> DeleteDictionary(int dictionaryId, CancellationToken cancellationToken)
    {
        if (dictionaryId <= 0) return BadRequest("O identificador do dicionário deve ser positivo.");
        var affected = await repository.DeleteDictionaryAsync(dictionaryId, cancellationToken);
        return affected == 0 ? NotFound() : Ok(new { recordsAffected = affected });
    }

    private static bool IsUniqueViolation(SqlException exception) => exception.Number is 2601 or 2627;
    private static bool IsForeignKeyViolation(SqlException exception) => exception.Number == 547;
}
