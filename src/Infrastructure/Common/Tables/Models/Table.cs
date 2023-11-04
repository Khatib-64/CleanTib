namespace CleanTib.Infrastructure.Common;

public class Table
{
    public string? Title { get; set; }
    public List<string> Columns { get; set; } = default!;
    public List<List<string>> Rows { get; set; } = default!;
}