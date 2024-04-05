using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DelimaIt.Core.UseCases;
using DeLimaIt.Dictionary.Application.Features.Configuration.Models;
using DeLimaIt.Dictionary.Api.Transport.V1;
using DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities;
using System.Reflection;

namespace DeLimaIt.Dictionary.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly UseCaseHandlerBase<ConfigurationDictionaryGetInput, List<ConfigurationDictionaryGetOutput>> _configurationDictionaryGetHandler;
        private readonly UseCaseHandlerBase<ConfigurationDictionaryValueGetInput, List<ConfigurationDictionaryValueGetOutput>> _configurationDictionaryValueGetHandler;
        private readonly UseCaseHandlerBase<ConfigurationDictionaryInput, ConfigurationDictionaryOutput> _configurationDictionaryHandler;
        private readonly UseCaseHandlerBase<ConfigurationModuleInput,List<ConfigurationModuleOutput>> _configurationModuleHandler;

        public ConfigurationController(UseCaseHandlerBase<ConfigurationDictionaryGetInput, List<ConfigurationDictionaryGetOutput>> configurationDictionaryGetHandler, UseCaseHandlerBase<ConfigurationDictionaryValueGetInput, List<ConfigurationDictionaryValueGetOutput>> configurationDictionaryValueGetHandler, UseCaseHandlerBase<ConfigurationDictionaryInput, ConfigurationDictionaryOutput> configurationDictionaryHandler, UseCaseHandlerBase<ConfigurationModuleInput, List<ConfigurationModuleOutput>> configurationModuleHandler)
        {
            _configurationDictionaryGetHandler = configurationDictionaryGetHandler;
            _configurationDictionaryValueGetHandler = configurationDictionaryValueGetHandler;
            _configurationDictionaryHandler = configurationDictionaryHandler;
            _configurationModuleHandler = configurationModuleHandler;
        }

        [HttpGet("Dictionaries/{moduleId}", Name = "GetDictionaries")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ConfigurationDictionaryGetOutput>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDictionaries([FromRoute] int moduleId, CancellationToken cancellationToken)
        {
            var input = new ConfigurationDictionaryGetInput(moduleId);
            var output = await _configurationDictionaryGetHandler.ExecuteAsync(input, cancellationToken);
            if (output.IsValid)
            {
                return Ok(output.GetResult());
            }
            return BadRequest(output);
        }
        [HttpGet("Dictionaries/Values/{DictionaryId}", Name = "GetDictionaryValues")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ConfigurationDictionaryValueGetOutput>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDictionaryValues([FromRoute] int DictionaryId, CancellationToken cancellationToken)
        {
            var input = new ConfigurationDictionaryValueGetInput(DictionaryId);
            var output = await _configurationDictionaryValueGetHandler.ExecuteAsync(input, cancellationToken);
            if (output.IsValid)
            {
                return Ok(output.GetResult());
            }
            return BadRequest(output);
        }


        [HttpDelete("Dictionaries/Values/{DictionaryId}/{DictionaryValueKey}", Name = "DeleteDictionaryValues")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ConfigurationDictionaryOutput>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDictionaryValues([FromRoute] int DictionaryId, [FromRoute] string DictionaryValueKey, CancellationToken cancellationToken)
        {
            var input = new ConfigurationDictionaryInput(OperationType.Delete, DictionaryId, DictionaryValueKey, "");
            var output = await _configurationDictionaryHandler.ExecuteAsync(input, cancellationToken);
            if (output.IsValid)
            {
                return Ok(output.GetResult());
            }
            return BadRequest(output);
        }
        [HttpPut("Dictionaries/Values/{DictionaryId}", Name = "UpdateDictionaryValues")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateDictionaryValues([FromRoute] int DictionaryId, [FromBody] DictionaryValueRequest Dictionary, CancellationToken cancellationToken)
        {
            var input = new ConfigurationDictionaryInput(OperationType.Update, DictionaryId, Dictionary.Key ,  Dictionary.Value);
            var output = await _configurationDictionaryHandler.ExecuteAsync(input, cancellationToken);
            if (output.IsValid)
            {
                return Ok(output.GetResult());
            }
            return BadRequest(output);
        }
        [HttpPost("Dictionaries/Values/{DictionaryId}", Name = "InsertDictionaryValues")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertDictionaryValues([FromRoute] int DictionaryId, [FromBody] DictionaryValueRequest Dictionary, CancellationToken cancellationToken)
        {
            var input = new ConfigurationDictionaryInput(OperationType.Insert, DictionaryId, Dictionary.Key, Dictionary.Value);
            var output = await _configurationDictionaryHandler.ExecuteAsync(input, cancellationToken);
            if (output.IsValid)
            {
                return Ok(output.GetResult());
            }
            return BadRequest(output);
        }
        [HttpGet("Modules/{moduleId}", Name = "GetModules")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ConfigurationModuleOutput>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetModules([FromRoute] int moduleId, CancellationToken cancellationToken)
        {
            var input = new ConfigurationModuleInput();
            input.Entity = new ConfigurationModuleEntity(); 
            input.Entity.ModuleId = moduleId;
            var output = await _configurationModuleHandler.ExecuteAsync(input, cancellationToken);
            if (output.IsValid)
            {
                return Ok(output.GetResult());
            }
            return BadRequest(output);
        }
    }
}
