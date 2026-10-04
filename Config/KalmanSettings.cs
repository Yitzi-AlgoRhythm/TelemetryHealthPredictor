namespace TelemetryHealthPredictor.Config
{
    public class KalmanSettings
    {
        public double ForgettingFactor { get; set; }

        public string MeasuredVariablesPath { get; set; } = string.Empty;
        public string StateVariablesPath { get; set; } = string.Empty;
        public string X0Path { get; set; } = string.Empty;
        public string P0Path { get; set; } = string.Empty;
        public string q0VectorPath { get; set; } = string.Empty;
        public string Q0MatrixPath { get; set; } = string.Empty;
        public string r0VectorPath { get; set; } = string.Empty;
        public string RMatrixPath { get; set; } = string.Empty;
        public string FPath { get; set; } = string.Empty;
        public string HPath { get; set; } = string.Empty;
    }
}