using System.Runtime.Serialization;


namespace DelimaIt.Core.UseCases.Outputs.Exceptions
{
    [Serializable]
    public class ValidationExceptionNullException : Exception
    {
        public ValidationExceptionNullException(string message) : base(message) { }
        public ValidationExceptionNullException(string message, Exception innerException) : base(message, innerException) { }
        public ValidationExceptionNullException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }
}
