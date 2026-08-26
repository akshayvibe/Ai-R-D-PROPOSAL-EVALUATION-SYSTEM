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
}
