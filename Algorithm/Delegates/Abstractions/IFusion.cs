using TelemetryHealthPredictor.Data;

namespace TelemetryHealthPredictor.Algorithm.Delegates.Abstractions
{
    public interface IFusion
    {
        public AlgorithmResult FusionDelegate(Tuple<MahalanobisResult, CusumResult> combinedResult);
    }
}
