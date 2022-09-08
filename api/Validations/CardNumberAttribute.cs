using System.ComponentModel.DataAnnotations;

namespace api.Validations;

// Validação se existe espaços em branco.
public class CardNumberAttribute : ValidationAttribute // Validação se possui caracter vazio
{
    public string EmptyChar { get; set; }

    public CardNumberAttribute(string eChar)
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
