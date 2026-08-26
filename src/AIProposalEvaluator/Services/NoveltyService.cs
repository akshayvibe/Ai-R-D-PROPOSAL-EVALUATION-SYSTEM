using System.Text.RegularExpressions;
using AIProposalEvaluator.Models;

namespace AIProposalEvaluator.Services;

public interface INoveltyService
{
    (double NoveltyScore, List<SimilarProject> SimilarProjects) Analyze(string proposalText);
    void LoadPastProjects(string csvPath);
}

public class NoveltyService : INoveltyService
{
    private List<(string Project, string Url, string Text)> _pastProjects = new();
    private readonly ILogger<NoveltyService> _logger;

    public NoveltyService(ILogger<NoveltyService> logger)
    {
        _logger = logger;
    }

    public void LoadPastProjects(string csvPath)
    {
        _pastProjects.Clear();

        if (!File.Exists(csvPath))
        {
            _logger.LogWarning("past_projects.csv not found at {Path}", csvPath);

            _pastProjects.Add(("IoT + AI Smart Energy Monitoring", "https://arxiv.org/abs/xxxx", "iot ai smart energy monitoring sensor network"));
            _pastProjects.Add(("AI-based Crop Disease Detection", "https://ieeexplore.ieee.org/xxxxx", "ai crop disease detection image classification agriculture"));
            _pastProjects.Add(("Smart Traffic Optimization using DL", "https://arxiv.org/abs/yyyy", "smart traffic optimization deep learning computer vision"));
            _pastProjects.Add(("Blockchain based Secure Voting System", "", "blockchain secure voting electronic election cryptography"));
            _pastProjects.Add(("Federated Learning for Healthcare", "", "federated learning privacy healthcare medical data"));

            return;
        }

        var lines = File.ReadAllLines(csvPath);
        if (lines.Length < 2) return;

        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(',', 2);
            if (parts.Length == 0) continue;

            var project = parts[0].Trim().Trim('"');
            var url = parts.Length > 1 ? parts[1].Trim().Trim('"') : "";

            if (string.IsNullOrWhiteSpace(project)) continue;

            _pastProjects.Add((project, url, project.ToLowerInvariant()));
        }

        _logger.LogInformation(
            "Loaded {Count} past projects for novelty benchmarking",
            _pastProjects.Count);
    }

}
