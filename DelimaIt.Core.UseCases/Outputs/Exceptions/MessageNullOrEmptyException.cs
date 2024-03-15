using System.Runtime.Serialization;

namespace DelimaIt.Core.UseCases.Outputs.Exceptions
{
    [Serializable]
    public class MessageNullOrEmptyException : Exception
    {
        public MessageNullOrEmptyException(string message) : base(message) { }
        public MessageNullOrEmptyException(string message, Exception innerException) : base(message, innerException) { }
        public MessageNullOrEmptyException(SerializationInfo info, StreamingContext context): base(info, context) { }
    }
}
