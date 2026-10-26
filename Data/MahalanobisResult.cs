namespace TelemetryHealthPredictor.Data
{
    public class MahalanobisResult
    {
        public required double DistanceSquared { get; init; }
        public required Dictionary<string, double> OutlierParameters { get; init; }
        public required double DeltaT { get; init; }
    }
}
