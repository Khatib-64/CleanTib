namespace CleanTib.Application.Common.Models;

public class Search
{
    public List<string> Fields { get; set; } = [];
    public string? Keyword { get; set; }
}