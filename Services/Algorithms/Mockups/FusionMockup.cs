using TelemetryHealthPredictor.Model.AlgorithmResultObjects;
using TelemetryHealthPredictor.Services.Algorithms.Abstractions;

namespace TelemetryHealthPredictor.Services.Algorithms.Mockups
{
    public class FusionMockup : IFusion
    {
        public AlgorithmResult FusionDelegate(Tuple<MahalanobisResult, Dictionary<string, double>> combinedResult)
        {
            Console.WriteLine("Fusing results");

            return new AlgorithmResult()
            {
                HealthScore = 100
            };
        }
    }
}
