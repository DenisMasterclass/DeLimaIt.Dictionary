using DelimaIt.Core.UseCases.Outputs;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Serilog.Core.Enrichers;
using Serilog.Events;
using SerilogTimings;

namespace DelimaIt.Core.UseCases
{
    public abstract class UseCaseHandlerBase<TRequest, TResponse> : IUseCase<TRequest, TResponse> where TRequest : notnull, IRequest<TResponse> where TResponse : notnull
    {
        private readonly ILogger<UseCaseHandlerBase<TRequest, TResponse>> _logger;
        private readonly IValidator<TRequest> _validator;

        protected UseCaseHandlerBase(ILogger<UseCaseHandlerBase<TRequest, TResponse>> logger, IValidator<TRequest> validator = null)
        {
            _logger = logger;
            _validator = validator;
        }
        public async Task<Output<TResponse>> ExecuteAsync(TRequest request, CancellationToken cancellationToken)
        {
            using (LogContext.Push(new PropertyEnricher("CorrelationId", request, false)))
            {
                using (Operation.At(LogEventLevel.Information).Time("Timing for " + GetType().Name))
                {
                    _logger.LogInformation("Start : {HandlerName} ", GetType().Name);
                    if (_validator != null)
                    {
                        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
                        if (!validationResult.IsValid)
                        {
                            Output<TResponse> output = new Output<TResponse>();
                            output.AddValidationResult(validationResult);
                            _logger.LogInformation("Finalize : [{NameRequest}] : {Success}", typeof(TRequest).Name, output.IsValid);
                            return output;
                        }
                    }
                }
                Output<TResponse> output2 = await HandleAsync(request, cancellationToken);
                _logger.LogInformation("Finalize : [{NameRequest}] : {Success}", typeof(TRequest).Name, output2.IsValid);
                return output2;
            }
        }
        protected abstract Task<Output<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken);
    }
}
