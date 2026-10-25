namespace TelemetryHealthPredictor.Data
{
    public class AlgorithmResult
    {
        public required int HealthScore { get; init; }
        public Dictionary<string, double>? MahalanobisAnomalies { get; init; }
        public Dictionary<string, double>? CusumAnomalies { get; init; }

        public AlgorithmResult
        (
            int healthScore, 
            Dictionary<string, double>? mahalanobisAnomalies, 
            Dictionary<string, double>? cusumAnomalies
        )
        {
            HealthScore = healthScore;
            MahalanobisAnomalies = mahalanobisAnomalies;
            CusumAnomalies = cusumAnomalies;
        }
    }
}
