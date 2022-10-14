using System.ComponentModel.DataAnnotations;

namespace api.Validations;

public class CPFAttributePattern : ValidationAttribute
{
    public CPFAttributePattern()
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value.GetType() != typeof(string)) return new ValidationResult("Erro de tipo!");
            
            var cpfLengthInvalid = ((string)value).Length != 11;
            var cpfDigitsInvalid = !((string)value).All(char.IsDigit);
            
            return cpfDigitsInvalid == true || cpfLengthInvalid == true
                    ? new ValidationResult("Atenção! Padrão inválido.")
                    : ValidationResult.Success;
        }
}
