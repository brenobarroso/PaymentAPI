using System.ComponentModel.DataAnnotations;

namespace api.Validations
{
    public class CPFAttribute : ValidationAttribute
    {
        public string EmptyChar { get; set; }

        public CPFAttribute(string eChar)
        {
            EmptyChar = eChar;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            return ((string)value).Contains(EmptyChar)
                ? new ValidationResult("Atenção! Espaço em  detectado.")
                : ValidationResult.Success;
        }
    }
}