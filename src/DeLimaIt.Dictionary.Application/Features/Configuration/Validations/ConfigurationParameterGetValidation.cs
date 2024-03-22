using DeLimaIt.Dictionary.Application.Features.Configuration.Models;
using FluentValidation;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.Validations
{
    public sealed class ConfigurationDictionaryGetValidation : AbstractValidator<ConfigurationDictionaryGetInput>
    {
        public ConfigurationDictionaryGetValidation()
        {
            ValidateValue();
        }
        public void ValidateValue()
        {
            RuleFor(c => c.ModuleId)
                .GreaterThanOrEqualTo(0).WithMessage("Type invalid. The value must be greater than 0!");
        }
    }
}
