using TelemetryHealthPredictor.Model.AlgorithmResultObjects;
using TelemetryHealthPredictor.Services.Algorithms.Abstractions;

namespace TelemetryHealthPredictor.Services.Algorithms.Mockups
{
    public class MahalanobisMockup : IMahalanobis
    {
        public MahalanobisResult MahalanobisDelegate(FilteredResult filteredResult)
        {
            return new MahalanobisResult()
            {
                DistanceScore = 0,
                Anomalies = []
            };
        }
    }
}
