namespace TelemetryHealthPredictor.Data
{
    public class CusumResult
    {
        public required List<string> AnomalousParameterNames { get; set; }
        public required List<double> AmountsAboveThresholds { get; set; }
        public required List<double> AnomalousParameterValues { get; set; }
    }
}
