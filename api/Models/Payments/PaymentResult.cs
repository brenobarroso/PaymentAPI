namespace api.Models;

public class PaymentResult
{
    public int Id { get; set; } // Identificador NSU
    public DateTime TransationDate { get; set; } // Data da transação
    public DateTime? ApprovalDate { get; set; } // Data da aprovação (caso ocorra)
    public DateTime? DisapprovalDate { get; set; } // Data de reprovação (caso ocorra)
    public bool Confirmation { get; set; } // Confirmação da adquirente
    public decimal GrossValue { get; set; } // Valor bruto da transação
    public decimal? NetValue { get; set; } // Valor líquido da transação (descontado taxa)
    public decimal? FlatRate { get; set; } // Taxa fixa cobrada
    public string CardNumber { get; set; } // 4 ultimos digitos do cartão
    public ICollection<InstallmentResult> Installments { get; set; } // Lista das parcelas
}