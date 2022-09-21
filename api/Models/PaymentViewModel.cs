using System.ComponentModel.DataAnnotations;
using api.Models;
using api.Validations;
using PaymentAPI.Models;

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
