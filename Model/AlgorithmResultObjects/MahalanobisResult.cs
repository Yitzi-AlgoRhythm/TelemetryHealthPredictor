namespace TelemetryHealthPredictor.Model.AlgorithmResultObjects
{
    public class MahalanobisResult
    {
        public required double DistanceScore { get; init; }
        public required Dictionary<string, double> Anomalies { get; init; }
    }
}
