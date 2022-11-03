using System.ComponentModel.DataAnnotations;
using PaymentAPI.Models;

namespace api.Models.Movements;

public class MovementResult
{
    [Required]
    public int AccountId { get; set; } 

    [Required]
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public decimal NetValue { get; set; }

    [Required]
    public decimal? GrossValue { get; set; }

    [Required]
    public string Comments { get; set; } = " ";
}
