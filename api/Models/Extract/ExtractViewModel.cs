using System.ComponentModel.DataAnnotations;

namespace api.Models.Extract;

public class ExtractViewModel
{    
    [Required, Range(0, int.MaxValue)]
    public int Index { get; set; }
    public int Length { get; set; }
    public bool? JustIn { get; set; }
    public bool? JustOut { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
}
