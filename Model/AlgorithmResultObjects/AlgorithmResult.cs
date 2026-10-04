namespace TelemetryHealthPredictor.Model.AlgorithmResultObjects
{
    public class AlgorithmResult
    {
        public required int HealthScore { get; init; }
        public Dictionary<string, double>? MahalanobisAnomalies { get; init; }
        public Dictionary<string, double>? CusumAnomalies { get; init; }
    }
}
