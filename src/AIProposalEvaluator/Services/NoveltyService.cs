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
}
