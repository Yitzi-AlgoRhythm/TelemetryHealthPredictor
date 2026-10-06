using TelemetryHealthPredictor.Model.AlgorithmResultObjects;

namespace TelemetryHealthPredictor.Services.Algorithms.Abstractions
{
    public interface IFusion
    {
        public AlgorithmResult FusionDelegate(Tuple<MahalanobisResult, Dictionary<string, double>> combinedResult);
    }
}
