namespace RoverSearch.Models;

public class SearchResults
{
    public string Query { get; set; } = string.Empty;
    public List<Result> Results { get; set; } = new();
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
}
