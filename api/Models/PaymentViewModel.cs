using System.ComponentModel.DataAnnotations;
using api.Validations;

namespace api.Models;

public class PaymentViewModel
{
    [Required]
    [ValueAttribute]
    public float GrossValue { get; set; } // Valor bruto da transação.

    [Required]
    [CardNumberAttribute(eChar: " ")]
    [CardNumberPatternAttribute]
    public string CardNumber { get; set; } // Número do cartão.

    [Required]
    public int InstallmentQuantity { get; set; } = 1; // Quantidade de parcelas da transação.
}
