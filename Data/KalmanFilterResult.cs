using MathNet.Numerics.LinearAlgebra;

namespace TelemetryHealthPredictor.Data
{
    public class KalmanFilterResult
    {
        public Vector<double> ParameterZScores { get; init; }
        public int[] IndicesMeasured { get; init; }

        public KalmanFilterResult(Vector<double> parameterZScores, int[] indicesMeasured)
        {
            ParameterZScores = parameterZScores.Clone();
            IndicesMeasured = indicesMeasured;
        }
    }
}
