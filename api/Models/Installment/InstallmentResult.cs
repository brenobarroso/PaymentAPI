namespace api.Models;

public class InstallmentResult
{
    public int Id { get; set; }  // Id correspondente à transação.
    public decimal InstallmentGrossValue { get; set; } // Valor bruto da parcela.
    public decimal InstallmentNetValue { get; set; } // Valor líquido da parcela.
    public int InstallmentNumber { get; set; } // Número da parcela
    public DateTime ReceiptDate { get; set; } // Data de recebimento esperado da parcela.
}
