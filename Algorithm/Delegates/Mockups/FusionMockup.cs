using TelemetryHealthPredictor.Algorithm.Delegates.Abstractions;
using TelemetryHealthPredictor.Data;

namespace TelemetryHealthPredictor.Algorithm.Delegates.Mockups
{
    public class FusionMockup : IFusion
    {
        public AlgorithmResult FusionDelegate(Tuple<MahalanobisResult, CusumResult> combinedResult)
        {
            Console.WriteLine("Fusing results");

            return new AlgorithmResult()
            {
                HealthScore = 100
            };
        }
    }
}
