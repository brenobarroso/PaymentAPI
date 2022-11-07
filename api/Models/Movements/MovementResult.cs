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

    public decimal Value { get; set; }

    [Required]
    public string Comments { get; set; } = " ";
}
