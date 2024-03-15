using System.Runtime.Serialization;

namespace DelimaIt.Core.UseCases.Outputs.Exceptions
{
    [Serializable]
    public class ErrorMessageNullOrEmptyException : Exception
    {
        public ErrorMessageNullOrEmptyException(string message) : base(message) { }
        public ErrorMessageNullOrEmptyException(string message, Exception innerException) : base(message, innerException) { }
        public ErrorMessageNullOrEmptyException(SerializationInfo info, StreamingContext context): base(info, context) { }
    }
}
