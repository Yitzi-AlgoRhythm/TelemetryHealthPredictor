using TelemetryHealthPredictor.Algorithm.Delegates.Abstractions;
using TelemetryHealthPredictor.Data;

namespace TelemetryHealthPredictor.Algorithm.Delegates.Implementations
{
    public class Fusion : IFusion
    {
        public AlgorithmResult FusionDelegate(Tuple<MahalanobisResult, CusumResult> combinedResult)
        {
            throw new NotImplementedException();
        }
    }
}
