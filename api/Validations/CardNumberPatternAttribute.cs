using System.ComponentModel.DataAnnotations;

namespace api.Validations
{
    public class CardNumberPatternAttribute : ValidationAttribute
    {
        public CardNumberPatternAttribute()
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var cardLengthInvalid = ((string)value).Length != 16;
            var cardDigitsInvalid = !((string)value).All(char.IsDigit);
            
            return cardDigitsInvalid == true || cardLengthInvalid == true
                    ? new ValidationResult("Atenção! Padrão inválido.")
                    : ValidationResult.Success;
        }
    }
}