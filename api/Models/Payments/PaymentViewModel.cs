using System.ComponentModel.DataAnnotations;
using api.Validations;
using api.Validations.Value;

namespace api.Models;

public class PaymentViewModel
{
    [Required]
    [ValueAttribute]
    public decimal GrossValue { get; set; } // Valor bruto da transação.

    [Required]
    [CardNumberAttribute(eChar: " ")]
    [CardNumberPatternAttribute]
    public string CardNumber { get; set; } // Número do cartão.

    [Required]
    public int InstallmentQuantity { get; set; } = 1; // Quantidade de parcelas da transação.

    [Required]
    public string AccountNumber { get; set; } // Identificador da conta correspondente.
}
