namespace TelemetryHealthPredictor.Config.Settings
{
    public class MahalanobisSettings
    {
        public double StepSize { get; set; }
        public string CovarianceMatrixPath { get; set; } = string.Empty;
        public double DistanceSquaredThreshold { get; set; }
    }
}
