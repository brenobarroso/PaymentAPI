using System.ComponentModel.DataAnnotations;

namespace api.Validations.Withdraw;

// Validação se o valor bruto da transação existe e não é negativo.
public class WithdrawValue : ValidationAttribute // Validação se o valor bruto for passado mas for negativo.
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        return value == null || (decimal)value <= 0
            ? new ValidationResult(ErrorMessage = "Valor Negativo")
            : ValidationResult.Success;
    }
}
