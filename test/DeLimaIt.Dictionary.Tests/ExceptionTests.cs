using System.Runtime.Serialization;
using DelimaIt.Core.UseCases.Outputs.Exceptions;

namespace DeLimaIt.Dictionary.Tests;

public sealed class ExceptionTests
{
    [Fact]
    public void Domain_exceptions_preserve_message_and_inner_exception()
    {
        var inner = new InvalidOperationException("inner");
        Assert.Equal("message", new ErrorMessageNullOrEmptyException("message").Message);
        Assert.Same(inner, new ErrorMessageNullOrEmptyException("message", inner).InnerException);
        Assert.Equal("message", new MessageNullOrEmptyException("message").Message);
        Assert.Same(inner, new MessageNullOrEmptyException("message", inner).InnerException);
        Assert.Equal("message", new ResultNullException("message").Message);
        Assert.Same(inner, new ResultNullException("message", inner).InnerException);
        Assert.Equal("message", new ValidationExceptionNullException("message").Message);
        Assert.Same(inner, new ValidationExceptionNullException("message", inner).InnerException);
    }

    [Fact]
    public void Serialization_constructors_reject_missing_serialization_data()
    {
#pragma warning disable SYSLIB0051
        var info = new SerializationInfo(typeof(Exception), new FormatterConverter());
        var context = new StreamingContext(StreamingContextStates.All);
        Assert.Throws<SerializationException>(() => new ErrorMessageNullOrEmptyException(info, context));
        Assert.Throws<SerializationException>(() => new MessageNullOrEmptyException(info, context));
        Assert.Throws<SerializationException>(() => new ResultNullException(info, context));
        Assert.Throws<SerializationException>(() => new ValidationExceptionNullException(info, context));
#pragma warning restore SYSLIB0051
    }
}
