using System.ComponentModel.DataAnnotations;

namespace api.Validations
{
    public class CPFAttribute : ValidationAttribute
    {
        public string EmptyChar { get; set; }

        public CPFAttribute()
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(String.IsNullOrWhiteSpace((string)value))
                return new ValidationResult("Espaço detectado!");
            return ValidationResult.Success;

            // return ((string)value).Contains(EmptyChar)
            //     ? new ValidationResult("Atenção! Espaço em  detectado.")
            //     : ValidationResult.Success;
        }
    }
}