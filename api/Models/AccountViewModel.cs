using System.ComponentModel.DataAnnotations;
using api.Validations;

namespace api.Models;

public class AccountViewModel
{
    [Required]
    [CPFAttribute(eChar: " ")]
    [CPFAttributePattern]
    public string CPF { get; set; } = " ";

    [Required]
    public string Agency { get; set; } = " ";

    [Required]
    [StringLength(50,MinimumLength=3)]
    public string HolderName { get; set; } = " ";
}
