namespace api.Models.Extract;

public class ExtractResult
{
    public int Index { get; set; }
    public int Length { get; set; }
    public List<string>? Itens  { get; set; }
    public long Count { get; set; }
}
