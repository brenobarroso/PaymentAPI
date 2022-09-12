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
            var cardLength = ((string)value).Length != 16;
            var cardDigits = ((string)value).All(char.IsDigit);
            
            return cardDigits == true || cardLength == false
                    ? new ValidationResult("Atenção! Padrão inválido.")
                    : ValidationResult.Success;
        }
    }
}