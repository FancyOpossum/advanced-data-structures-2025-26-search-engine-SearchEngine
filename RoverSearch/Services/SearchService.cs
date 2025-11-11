using System.Diagnostics;
using System.Text.RegularExpressions;
using RoverSearch.Models;

namespace RoverSearch.Services;

public class SearchService
{
    private string path = @".\\Data\\";

    public SearchResults Search(string query)
    {
        var sw = new Stopwatch();
        sw.Start();

        var results = new List<Result>();
        var words = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (string file in Directory.GetFiles(path))
        {
            string rawText = File.ReadAllText(file);
            string content = rawText.ToLower();

            // Extract metadata
            var lines = rawText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            string title = lines.FirstOrDefault(l => l.StartsWith("title:", StringComparison.OrdinalIgnoreCase))?
                              .Replace("title:", "", StringComparison.OrdinalIgnoreCase).Trim() ?? "Unknown Title";
            string season = lines.FirstOrDefault(l => l.StartsWith("season:", StringComparison.OrdinalIgnoreCase))?
                              .Replace("season:", "", StringComparison.OrdinalIgnoreCase).Trim() ?? "?";
            string episode = lines.FirstOrDefault(l => l.StartsWith("episode:", StringComparison.OrdinalIgnoreCase))?
                              .Replace("episode:", "", StringComparison.OrdinalIgnoreCase).Trim() ?? "?";

            // Body text starts after metadata
            string scriptBody = string.Join('\n', lines.SkipWhile(l => !string.IsNullOrWhiteSpace(l)).Skip(1));

            // Word-based match
            bool allWordsPresent = words.All(word => content.Contains(word));
            if (!allWordsPresent) continue;

            // Scoring (relevance)
            int score = 0;
            foreach (var word in words)
                score += Regex.Matches(content, $@"\b{Regex.Escape(word)}\b").Count;

            string snippet = GetSnippet(scriptBody.ToLower(), words[0]);

            results.Add(new Result
            {
                Filename = Path.GetFileName(file),
                EpisodeTitle = title,
                Snippet = snippet,
                Score = score,
                Description = $"Season {season}, Episode {episode}"
            });
        }

        results = results.OrderByDescending(r => r.Score).ToList();

        sw.Stop();

        return new SearchResults
        {
            Query = query,
            Results = results,
            Duration = sw.Elapsed
        };
    }

    private string GetSnippet(string content, string firstWord)
    {
        int index = content.IndexOf(firstWord);
        if (index < 0)
            return content.Substring(0, Math.Min(200, content.Length)) + "...";

        int start = Math.Max(0, index - 50);
        int length = Math.Min(200, content.Length - start);
        return content.Substring(start, length) + "...";
    }
}
