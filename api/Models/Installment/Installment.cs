using System.ComponentModel.DataAnnotations;
using PaymentAPI.Models;

namespace api.Models;

public class Installment
{
    [Required]
    public Payment Payment { get; set; }

    [Required]
    public int Id { get; set; }  // Id correspondente à transação.

    [Required]
    public decimal InstallmentGrossValue { get; set; } // Valor bruto da parcela.

    [Required]
    public decimal InstallmentNetValue { get; set; } // Valor líquido da parcela.

    [Required]
    public int InstallmentNumber { get; set; } // Número da parcela

    [Required]
    public DateTime ReceiptDate { get; set; } // Data de recebimento esperado da parcela.
}