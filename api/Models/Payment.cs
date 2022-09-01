using System;
using System.ComponentModel.DataAnnotations;
using PaymentAPI.Controllers;

namespace PaymentAPI.Models
{
    public class Payment
    {
        [Required]
        public int Id { get; set; } // Identificador NSU
        [Required]
        public DateTime TransationDate { get; set; } = DateTime.UtcNow;// Data da transação
        public DateTime? ApprovalDate { get; set; } = null;// Data da aprovação (caso ocorra)
        public DateTime? DisapprovalDate { get; set; } = null;// Data de reprovação (caso ocorra)
        public bool Confirmation { get; set; } // Confirmação da adquirente
        [Required]
        [ValueAttribute]
        public float GrossValue { get; set; } // Valor bruto da transação
        public float? NetValue { get; set; } = null; // Valor líquido da transação (descontado taxa)

        [Required (ErrorMessage = "Atenção, taxa não adicionada!")]
        public float? FlatRate { get; set; } = 0.9f;// Taxa fixa cobrada

        [Required (ErrorMessage = "Atenção, número do cartão não adicionado!")]
        [CardNumberAttribute(eChar:" ")]
        public string CardNumber { get; set; } = " ";// 4 ultimos digitos do cartão

    }
}