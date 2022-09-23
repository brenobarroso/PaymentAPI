using System.ComponentModel.DataAnnotations;
using api.Validations;

namespace api.Models;

public class PaymentViewModel
{
    [Required]
    [ValueAttribute]
    public float GrossValue { get; set; }

    [Required]
    [CardNumberAttribute(eChar: " ")]
    [CardNumberPatternAttribute]
    public string CardNumber { get; set; }
}
