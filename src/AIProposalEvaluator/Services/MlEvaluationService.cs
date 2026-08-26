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
}
