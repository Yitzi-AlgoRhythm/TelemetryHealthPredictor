namespace TelemetryHealthPredictor.Config.Settings
{
    public class KalmanSettings
    {
        public double ForgettingFactor { get; set; }

        public string AMatrixPath { get; set; } = string.Empty;
        public string GMatrixPath { get; set; } = string.Empty;
        public string R0MatrixPath { get; set; } = string.Empty;
        public string P0MatrixPath { get; set; } = string.Empty;
        public string QcVectorPath { get; set; } = string.Empty;
        public string X0VectorPath { get; set; } = string.Empty;
    }
}