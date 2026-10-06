using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Model.AlgorithmResultObjects;
using TelemetryHealthPredictor.Services.Algorithms.Abstractions;

namespace TelemetryHealthPredictor.Services.Algorithms.Mockups
{
    public class KalmanFilterMockup : IKalmanFilter
    {
        public FilteredResult KalmanFilterDelegate(KeyValuePair<string, double> parameterValue)
        {
            Console.WriteLine($"Running Kalman Filter on {parameterValue}");

            VectorBuilder<double> vectorBuilder = Vector<double>.Build;
            MatrixBuilder<double> matrixBuilder = Matrix<double>.Build;

            return new FilteredResult()
            {
                FilteredValues = vectorBuilder.Dense(97),
                CovarianceMatrix = matrixBuilder.Dense(97, 97),
                Means = vectorBuilder.Dense(97),
                StandardDeviations = vectorBuilder.Dense(97)
            };
        }
    }
}
