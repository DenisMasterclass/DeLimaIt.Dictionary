using DeLimaIt.Dictionary.Application.Features.Configuration.Models;
using FluentValidation;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.Validations
{
    public sealed class ConfigurationParameterValueGetValidation : AbstractValidator<ConfigurationParameterValueGetInput>
    {
        public ConfigurationParameterValueGetValidation()
        {
            ValidateValue();
        }
        public void ValidateValue()
        {
            RuleFor(c => c.ParameterId)
                .GreaterThanOrEqualTo(0).WithMessage("Type invalid. The value must be greater than 0!");
        }
    }
}
