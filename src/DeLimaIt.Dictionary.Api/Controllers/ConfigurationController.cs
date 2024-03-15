using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DelimaIt.Core.UseCases;
using DeLimaIt.Dictionary.Application.Features.Configuration.Models;
using DeLimaIt.Dictionary.Api.Transport.V1;

namespace DeLimaIt.Dictionary.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly UseCaseHandlerBase<ConfigurationParameterGetInput, List<ConfigurationParameterGetOutput>> _configurationParameterGetHandler;
        private readonly UseCaseHandlerBase<ConfigurationParameterValueGetInput, List<ConfigurationParameterValueGetOutput>> _configurationParameterValueGetHandler;
        private readonly UseCaseHandlerBase<ConfigurationParameterInput, ConfigurationParameterOutput> _configurationParameterHandler;

        public ConfigurationController(UseCaseHandlerBase<ConfigurationParameterGetInput, List<ConfigurationParameterGetOutput>> configurationParameterGetHandler, UseCaseHandlerBase<ConfigurationParameterValueGetInput, List<ConfigurationParameterValueGetOutput>> configurationParameterValueGetHandler, UseCaseHandlerBase<ConfigurationParameterInput, ConfigurationParameterOutput> configurationParameterHandler)
        {
            _configurationParameterGetHandler = configurationParameterGetHandler;
            _configurationParameterValueGetHandler = configurationParameterValueGetHandler;
            _configurationParameterHandler = configurationParameterHandler;
        }

        [HttpGet("Parameters/{moduleId}", Name = "GetParameters")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ConfigurationParameterGetOutput>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetParameters([FromRoute] int moduleId, CancellationToken cancellationToken)
        {
            var input = new ConfigurationParameterGetInput(moduleId);
            var output = await _configurationParameterGetHandler.ExecuteAsync(input, cancellationToken);
            if (output.IsValid)
            {
                return Ok(output.GetResult());
            }
            return BadRequest(output);
        }
        [HttpGet("Parameters/Values/{parameterId}", Name = "GetParameterValues")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ConfigurationParameterValueGetOutput>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetParameterValues([FromRoute] int parameterId, CancellationToken cancellationToken)
        {
            var input = new ConfigurationParameterValueGetInput(parameterId);
            var output = await _configurationParameterValueGetHandler.ExecuteAsync(input, cancellationToken);
            if (output.IsValid)
            {
                return Ok(output.GetResult());
            }
            return BadRequest(output);
        }


        [HttpDelete("Parameters/Values/{parameterId}/{parameterValueKey}", Name = "DeleteParameterValues")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ConfigurationParameterOutput>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteParameterValues([FromRoute] int parameterId, [FromRoute] string parameterValueKey, CancellationToken cancellationToken)
        {
            var input = new ConfigurationParameterInput(OperationType.Delete, parameterId, parameterValueKey, "");
            var output = await _configurationParameterHandler.ExecuteAsync(input, cancellationToken);
            if (output.IsValid)
            {
                return Ok(output.GetResult());
            }
            return BadRequest(output);
        }
        [HttpPut("Parameters/Values/{parameterId}/{parameterValueKey}", Name = "UpdateParameterValues")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateParameterValues([FromRoute] int parameterId, [FromBody] ParameterValueRequest parameter, CancellationToken cancellationToken)
        {
            var input = new ConfigurationParameterInput(OperationType.Update, parameterId, parameter.Key ,  parameter.Value);
            var output = await _configurationParameterHandler.ExecuteAsync(input, cancellationToken);
            if (output.IsValid)
            {
                return Ok(output.GetResult());
            }
            return BadRequest(output);
        }
        [HttpPost("Parameters/Values/{parameterId}/{parameterValueKey}", Name = "InsertParameterValues")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertParameterValues([FromRoute] int parameterId, [FromBody] ParameterValueRequest parameter, CancellationToken cancellationToken)
        {
            var input = new ConfigurationParameterInput(OperationType.Insert, parameterId, parameter.Key, parameter.Value);
            var output = await _configurationParameterHandler.ExecuteAsync(input, cancellationToken);
            if (output.IsValid)
            {
                return Ok(output.GetResult());
            }
            return BadRequest(output);
        }
    }
}
