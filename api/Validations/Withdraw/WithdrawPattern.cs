using System.ComponentModel.DataAnnotations;

namespace api.Validations.Withdraw
{
    public class WithdrawPattern : ValidationAttribute
    {
        public WithdrawPattern()
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var cardLengthInvalid = ((string)value).Length != 7;
            var cardDigitsInvalid = !((string)value).All(char.IsDigit);
            
            return cardDigitsInvalid == true || cardLengthInvalid == true
                    ? new ValidationResult("Atenção! Padrão inválido.")
                    : ValidationResult.Success;
        }
    }
}