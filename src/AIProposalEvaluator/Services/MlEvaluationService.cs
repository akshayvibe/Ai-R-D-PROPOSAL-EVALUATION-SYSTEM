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

    public List<double> EvaluateWithUncertainty(
        double noveltyScore,
        double financeScore,
        double technicalScore,
        double budget)
    {
        double novelty = Math.Clamp(noveltyScore / 100.0, 0.0, 1.0);
        double finance = Math.Clamp(financeScore / 100.0, 0.0, 1.0);
        double technical = Math.Clamp(technicalScore / 100.0, 0.0, 1.0);
        double budgetEfficiency = Math.Clamp(1.0 - (budget / 50_000_000.0), 0.0, 1.0);

        double baseScore =
            novelty * 42.0 +
            finance * 28.0 +
            technical * 20.0 +
            budgetEfficiency * 10.0;

        var predictions = new List<double>();

        for (int i = 0; i < 50; i++)
        {
            double noise = (_rng.NextDouble() - 0.5) * 12.0;
            double treeScore = baseScore + noise;

            if (_rng.NextDouble() < 0.15)
                treeScore += (_rng.NextDouble() - 0.5) * 8.0;

            predictions.Add(Math.Clamp(treeScore, 0.0, 100.0));
        }

        return predictions;
    }


    public ConfidenceBand EstimateConfidenceBand(IEnumerable<double> predictions)
    {
        var preds = predictions.ToArray();

        if (preds.Length == 0)
        {
            return new ConfidenceBand
            {
                Mean = 50,
                Lower = 40,
                Upper = 60,
                Std = 5,
                Confidence = 70
            };
        }

        double mean = preds.Average();
        double variance = preds.Select(p => (p - mean) * (p - mean)).Average();
        double std = Math.Sqrt(variance);

        double lower = Math.Max(0.0, mean - 1.96 * std);
        double upper = Math.Min(100.0, mean + 1.96 * std);
        double confidence = Math.Clamp(100.0 - (std * 4.0), 0.0, 100.0);

        return new ConfidenceBand
        {
            Mean = Math.Round(mean, 2),
            Lower = Math.Round(lower, 2),
            Upper = Math.Round(upper, 2),
            Std = Math.Round(std, 2),
            Confidence = Math.Round(confidence, 2)
        };
    }

}
