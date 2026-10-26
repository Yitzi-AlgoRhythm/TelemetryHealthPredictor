using MathNet.Numerics.LinearAlgebra;

namespace TelemetryHealthPredictor.Data
{
    public class KalmanFilterResult
    {
        public required Vector<double> ParameterZScores { get; init; }
        public required int[] IndicesMeasured { get; init; }
        public required double[] ParameterValues { get; init; }
        public required double DeltaT { get; init; }
    }
}
