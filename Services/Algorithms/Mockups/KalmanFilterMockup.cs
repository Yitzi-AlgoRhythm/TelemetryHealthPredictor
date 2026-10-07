using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Services.Algorithms.Abstractions;

namespace TelemetryHealthPredictor.Services.Algorithms.Mockups
{
    public class KalmanFilterMockup : IKalmanFilter
    {
        public Vector<double>[] KalmanFilterDelegate(KeyValuePair<string, double> parameterValue)
        {
            Console.WriteLine($"Running Kalman Filter on {parameterValue}");

            VectorBuilder<double> vectorBuilder = Vector<double>.Build;
            MatrixBuilder<double> matrixBuilder = Matrix<double>.Build;

            return new[] { vectorBuilder.Dense(97) };
        }
    }
}
