using System.ComponentModel.DataAnnotations;
using PaymentAPI.Models;

namespace api.Models;

public class Installment
{
    [Required]
    public Payment Payment { get; set; } // Id correspondente à transação.

    [Required]
    public int Id { get; set; }

    [Required]
    public float InstallmentGrossValue { get; set; } // Valor bruto da parcela.

    [Required]
    public float InstallmentNetValue { get; set; } // Valor líquido da parcela.

    [Required]
    public int InstallmentNumber { get; set; } // Número da parcela

    [Required]
    public DateTime ReceiptDate { get; set; } // Data de recebimento esperado da parcela.
}
