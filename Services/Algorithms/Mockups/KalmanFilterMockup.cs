using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;
using TelemetryHealthPredictor.Model.AlgorithmResultObjects;
using TelemetryHealthPredictor.Services.Algorithms.Abstractions;

namespace TelemetryHealthPredictor.Services.Algorithms.Mockups
{
    public class KalmanFilterMockup : IKalmanFilter
    {
        public KalmanFilterResult[] KalmanFilterDelegate(IncomingFlightParameter[] parameters)
        {
            Console.WriteLine($"Running Kalman Filter on {parameters}");

            VectorBuilder<double> vectorBuilder = Vector<double>.Build;
            MatrixBuilder<double> matrixBuilder = Matrix<double>.Build;

            return [new KalmanFilterResult(vectorBuilder.Dense(97))];
        }
    }
}
