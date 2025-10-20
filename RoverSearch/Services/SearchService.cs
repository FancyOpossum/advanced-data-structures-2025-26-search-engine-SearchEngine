using System.Diagnostics;
using RoverSearch.Models;

namespace RoverSearch.Services;

public class SearchService
{
    private string path = @".\Data\";

    public SearchService()
    {

    }

    /// <summary>
    /// Naive search implementation
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public SearchResults Search(string query)
    {
        var sw = new Stopwatch();
        sw.Start();

        var results = new List<Result>();

        foreach (string file in Directory.GetFiles(path))
        {
            if (File.ReadAllText(file).Contains(query))
            {
                var filename = Path.GetFileName(file);

                results.Add(new Result { Filename = filename});
            }
        }

        sw.Stop();

        return new SearchResults
        {
            Query = query,
            Results = results,
            Duration = sw.Elapsed
        };
    }
}
