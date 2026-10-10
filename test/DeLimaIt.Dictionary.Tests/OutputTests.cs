using DelimaIt.Core.UseCases.Outputs;
using DelimaIt.Core.UseCases.Outputs.Exceptions;
using FluentValidation.Results;

namespace DeLimaIt.Dictionary.Tests;

public sealed class OutputTests
{
    [Fact]
    public void Empty_output_starts_valid_with_empty_collections()
    {
        var output = new Output<string>();

        Assert.True(output.IsValid);
        Assert.Empty(output.Messages);
        Assert.Empty(output.ErrorMessages);
    }

    [Fact]
    public void Output_can_be_initialized_with_validity_or_result()
    {
        Assert.False(new Output<string>(false).IsValid);
        var withResult = new Output<string>("payload");
        Assert.True(withResult.IsValid);
        Assert.Equal("payload", withResult.GetResult());
    }

    [Fact]
    public void Add_result_rejects_null_and_replaces_existing_result()
    {
        var output = new Output<string>("old");
        output.AddResult("new");

        Assert.Equal("new", output.GetResult());
        Assert.Throws<ResultNullException>(() => output.AddResult(null!));
        Assert.Throws<ResultNullException>(() => new Output<string>().GetResult());
    }

    [Fact]
    public void Add_messages_supports_single_and_multiple_messages_and_rejects_empty()
    {
        var output = new Output<string>();
        output.AddMessages("one");
        output.AddMessages("two", "three");

        Assert.Equal(new[] { "one", "two", "three" }, output.Messages);
        Assert.True(output.IsValid);
        Assert.Throws<MessageNullOrEmptyException>(() => output.AddMessages(string.Empty));
        Assert.Throws<MessageNullOrEmptyException>(() => output.AddMessages((string)null!));
    }

    [Fact]
    public void Add_error_messages_marks_output_invalid_and_rejects_empty()
    {
        var output = new Output<string>();
        output.AddErrorMessages("bad input", "another issue");

        Assert.False(output.IsValid);
        Assert.Equal(new[] { "bad input", "another issue" }, output.ErrorMessages);
        Assert.Throws<ErrorMessageNullOrEmptyException>(() => output.AddErrorMessages(string.Empty));
        Assert.Throws<ErrorMessageNullOrEmptyException>(() => output.AddErrorMessages((string)null!));
    }

    [Fact]
    public void Validation_result_constructors_copy_errors_and_check_all_results()
    {
        var invalid = new ValidationResult(new[] { new ValidationFailure("Name", "required"), new ValidationFailure("Id", "invalid") });
        var single = new Output<string>(invalid);
        var multiple = new Output<string>(new[] { new ValidationResult(), invalid });

        Assert.False(single.IsValid);
        Assert.Equal(new[] { "required", "invalid" }, single.ErrorMessages);
        Assert.False(multiple.IsValid);
        Assert.Equal(new[] { "required", "invalid" }, multiple.ErrorMessages);
        Assert.Throws<ValidationExceptionNullException>(() => new Output<string>((ValidationResult)null!));
        Assert.Throws<ValidationExceptionNullException>(() => new Output<string>(new ValidationResult[] { null! }));
    }

    [Fact]
    public void Validation_result_methods_handle_valid_invalid_and_null_results()
    {
        var output = new Output<string>();
        output.AddValidationResult(new ValidationResult());
        Assert.True(output.IsValid);

        output.AddValidationResult(new ValidationResult(new[] { new ValidationFailure("Field", "failure") }));
        Assert.False(output.IsValid);
        Assert.Contains("failure", output.ErrorMessages);

        Assert.Throws<ValidationExceptionNullException>(() => output.AddValidationResult(null!));
        Assert.Throws<ValidationExceptionNullException>(() => Output<string>.CheckValidationResult(null!));
    }

    [Fact]
    public void Validation_state_accumulates_errors_and_informational_messages_do_not_change_it()
    {
        var output = new Output<string>();
        output.AddMessages("informational");
        Assert.True(output.IsValid);

        output.AddValidationResult(new ValidationResult(new[] { new ValidationFailure("Field", "invalid") }));
        output.AddValidationResult(new ValidationResult());
        output.AddMessages("still informational");

        Assert.False(output.IsValid);
        Assert.Equal(new[] { "invalid" }, output.ErrorMessages);
        Assert.Equal(new[] { "informational", "still informational" }, output.Messages);

        var explicitlyInvalid = new Output<string>(false);
        explicitlyInvalid.AddMessages("note");
        Assert.False(explicitlyInvalid.IsValid);
    }

    [Fact]
    public void Empty_error_message_addition_does_not_turn_an_invalid_output_valid()
    {
        var output = new Output<string>(false);
        output.AddErrorMessages();
        Assert.False(output.IsValid);
    }

    [Fact]
    public void Static_message_validators_reject_null_and_empty_values()
    {
        Output<string>.VerifyErrorMessage("error");
        Output<string>.VerifyMessage("message");
        Assert.Throws<ErrorMessageNullOrEmptyException>(() => Output<string>.VerifyErrorMessage(null!));
        Assert.Throws<ErrorMessageNullOrEmptyException>(() => Output<string>.VerifyErrorMessage(""));
        Assert.Throws<MessageNullOrEmptyException>(() => Output<string>.VerifyMessage(null!));
        Assert.Throws<MessageNullOrEmptyException>(() => Output<string>.VerifyMessage(""));
    }
}
