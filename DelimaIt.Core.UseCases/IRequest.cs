namespace DelimaIt.Core.UseCases
{
    public interface IRequest
    {
        Guid CorrelationId { get; set; }
        Guid TransactionId { get; set; }
    }
    public interface IRequest<T> : IRequest { }

}
