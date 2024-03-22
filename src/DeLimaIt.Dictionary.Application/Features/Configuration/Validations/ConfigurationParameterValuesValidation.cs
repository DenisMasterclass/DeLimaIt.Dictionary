using DeLimaIt.Dictionary.Application.Features.Configuration.Models;
using FluentValidation;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.Validations
{
    public sealed class ConfigurationDictionaryValuesValidation : AbstractValidator<ConfigurationDictionaryInput>
    {
        public ConfigurationDictionaryValuesValidation()
        {
            ValidateValue();
        }
        public void ValidateValue()
        {
            RuleFor(c => c.DictionaryId)
                .GreaterThanOrEqualTo(0).WithMessage("Type invalid. The value must be greater than 0!");
        }
    }
}
