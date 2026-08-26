using AIProposalEvaluator.Models;

namespace AIProposalEvaluator.Services;

public interface IMlEvaluationService
{
    List<double> EvaluateWithUncertainty(double noveltyScore, double financeScore, double technicalScore, double budget);
    ConfidenceBand EstimateConfidenceBand(IEnumerable<double> predictions);
    ShapResult GetShapLikeValues(double novelty, double finance, double technical, double budget);
    Dictionary<string, double> GetFeatureImportance();
    List<string> GenerateExplanation(double novelty, double finance, double technical);
}

/// <summary>
/// Pure C# ensemble-style evaluator that closely mirrors the original RandomForest behaviour.
/// Uses weighted scoring + controlled noise from multiple "virtual trees" for uncertainty.
/// </summary>
public class MlEvaluationService : IMlEvaluationService
{
    private readonly Random _rng = new(42);

    // Feature importances (mirrors typical RF output from the original tiny dataset)
    private static readonly Dictionary<string, double> FeatureImportances = new()
    {
        ["Novelty"] = 0.42,
        ["Financial Compliance"] = 0.28,
        ["Technical Feasibility"] = 0.20,
        ["Budget Efficiency"] = 0.10
    };
}
