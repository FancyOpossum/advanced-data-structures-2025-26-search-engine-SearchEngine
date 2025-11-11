using Microsoft.AspNetCore.Mvc;
using RoverSearch.Models;
using System.Diagnostics;
using RoverSearch.Services;

namespace RoverSearch.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SearchService _search;

    public HomeController(ILogger<HomeController> logger, SearchService search)
    {
        _logger = logger;
        _search = search;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Search(string query)
    {
        var results = _search.Search(query);

        return View(results);
    }
    public IActionResult ViewEpisode(string filename)
    {
        if (string.IsNullOrEmpty(filename))
            return NotFound();

        string path = Path.Combine(".\\Data\\", filename);
        if (!System.IO.File.Exists(path))
            return NotFound();

        string content = System.IO.File.ReadAllText(path);

        // Parse metadata (same logic as SearchService)
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        string title = lines.FirstOrDefault(l => l.StartsWith("title:", StringComparison.OrdinalIgnoreCase))?
                          .Replace("title:", "", StringComparison.OrdinalIgnoreCase).Trim() ?? "Unknown Title";
        string season = lines.FirstOrDefault(l => l.StartsWith("season:", StringComparison.OrdinalIgnoreCase))?
                          .Replace("season:", "", StringComparison.OrdinalIgnoreCase).Trim() ?? "?";
        string episode = lines.FirstOrDefault(l => l.StartsWith("episode:", StringComparison.OrdinalIgnoreCase))?
                          .Replace("episode:", "", StringComparison.OrdinalIgnoreCase).Trim() ?? "?";

        string scriptBody = string.Join('\n', lines.SkipWhile(l => !string.IsNullOrWhiteSpace(l)).Skip(1));

        var model = new EpisodeViewModel
        {
            Title = title,
            Season = season,
            Episode = episode,
            Script = scriptBody
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
