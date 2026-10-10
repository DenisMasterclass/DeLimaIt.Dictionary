using DelimaIt.Core.UseCases;
using DelimaIt.Core.UseCases.Outputs;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace DeLimaIt.Dictionary.Tests;

public sealed class UseCaseHandlerBaseTests
{
    [Fact]
    public async Task Handler_logs_the_request_correlation_id_and_returns_handler_result()
    {
        var correlationId = Guid.NewGuid();
        var sink = new InMemorySink();
        var previousLogger = Log.Logger;
        Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.Sink(sink).CreateLogger();
        try
        {
            var handler = new TestHandler();
            var output = await handler.ExecuteAsync(new TestRequest(correlationId, Guid.NewGuid()), CancellationToken.None);

            Assert.True(output.IsValid);
            Assert.Equal("handled", output.GetResult());
            var correlationEvent = Assert.Single(sink.Events.ToArray().Where(item =>
                item.Properties.TryGetValue("CorrelationId", out var value) && value is ScalarValue { Value: Guid loggedId } && loggedId == correlationId));
            Assert.IsType<ScalarValue>(correlationEvent.Properties["CorrelationId"]);
        }
        finally
        {
            Log.Logger = previousLogger;
        }
    }

    [Fact]
    public async Task Handler_can_execute_without_a_validator()
    {
        var handler = new TestHandler();
        var output = await handler.ExecuteAsync(new TestRequest(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        Assert.Equal("handled", output.GetResult());
    }

    [Fact]
    public async Task Handler_rejects_a_null_request()
    {
        var handler = new TestHandler();

        await Assert.ThrowsAsync<ArgumentNullException>(() => handler.ExecuteAsync(null!, CancellationToken.None));
    }

    private sealed class TestRequest(Guid correlationId, Guid transactionId) : IRequest<string>
    {
        public Guid CorrelationId { get; set; } = correlationId;
        public Guid TransactionId { get; set; } = transactionId;
    }

    private sealed class TestHandler : UseCaseHandlerBase<TestRequest, string>
    {
        public TestHandler() : base(NullLogger<UseCaseHandlerBase<TestRequest, string>>.Instance) { }

        protected override Task<Output<string>> HandleAsync(TestRequest request, CancellationToken cancellationToken)
            => Task.FromResult(new Output<string>("handled"));
    }

    private sealed class InMemorySink : ILogEventSink
    {
        public ConcurrentQueue<LogEvent> Events { get; } = new();
        public void Emit(LogEvent logEvent) => Events.Enqueue(logEvent);
    }
}
