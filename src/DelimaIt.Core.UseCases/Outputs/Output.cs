using DelimaIt.Core.UseCases.Outputs.Exceptions;
using FluentValidation.Results;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelimaIt.Core.UseCases.Outputs
{
    public class Output<T> where T : notnull
    {
        private List<string> _messages = new();
        private List<string> _errorMessages = new();
        private T _result = default!;
        private bool _hasResult;
        public IReadOnlyCollection<string> ErrorMessages => GetMessages(_errorMessages);
        public bool IsValid { get; protected set; }
        public IReadOnlyCollection<string> Messages => GetMessages(_messages);
        public Output(bool isValid = true)
        {
            IsValid = isValid;
        }
        public Output(T result)
        {
            IsValid = true;
            AddResult(result);
        }

        public Output(ValidationResult validationResult) : this()
        {
            ProcessValidationResult(validationResult);
        }

        public Output(IEnumerable<ValidationResult> validationResult) : this()
        {
            ArgumentNullException.ThrowIfNull(validationResult);
            ProcessValidationResult(validationResult.ToArray());
        }
        private static IReadOnlyCollection<string> GetMessages(List<string> messages)
        {
            return messages.AsReadOnly();
        }
        public static void CheckValidationResult(ValidationResult validationResult)
        {
            if (validationResult == null)
            {
                throw new ValidationExceptionNullException("Validation Result is null. Please verify.");
            }
        }
        public static void VerifyErrorMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ErrorMessageNullOrEmptyException("Error while trying to add string to ErrorMessage Collection. It is null or empty, please verify.");
            }
        }
        public static void VerifyMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new MessageNullOrEmptyException("Error while trying to add string to Message Collection. It is null or empty, please verify.");
            }
        }
        private void VerifyErrorsMessages(ValidationResult validationResult)
        {
            _errorMessages.AddRange(validationResult.Errors.Select((ValidationFailure e) => e.ErrorMessage).ToList());
        }

        protected virtual void VerifyValidity()
        {
            IsValid = IsValid && ErrorMessages.Count == 0;
        }
        public void AddErrorMessages(params string[] messages)
        {
            foreach (string text in messages)
            {
                VerifyErrorMessage(text);
                _errorMessages.Add(text);
                IsValid = false;
            }
        }
        public void AddMessages(string messages) => AddMessages(new[] { messages });
        public void AddMessages(params string[] messages)
        {
            foreach (string text in messages)
            {
                VerifyMessage(text);
                _messages.Add(text);
            }
        }

        public void AddResult(T result)
        {
            if (result == null)
            {
                throw new ResultNullException("Result object is null, please verify");
            }
            _result = result;
            _hasResult = true;
        }

        public virtual void AddValidationResult(ValidationResult validationResult)
        {
            CheckValidationResult(validationResult);
            IsValid = IsValid && validationResult.IsValid;
            VerifyErrorsMessages(validationResult);
            VerifyValidity();
        }

        private void ProcessValidationResult(params ValidationResult[] validationResults)
        {
            foreach (ValidationResult validationResult in validationResults)
            {
                CheckValidationResult(validationResult);
                AddValidationResult(validationResult);
            }
            VerifyValidity();
        }
        public T GetResult()
        {
            if (!_hasResult)
            {
                throw new ResultNullException("Result has not been set. Call AddResult before GetResult.");
            }
            return _result;
        }
    }
}
