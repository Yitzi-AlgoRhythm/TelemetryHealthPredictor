using MathNet.Numerics.LinearAlgebra;

namespace TelemetryHealthPredictor.Services.Algorithms.Abstractions
{
    public interface IKalmanFilter
    {
        public Vector<double>[] KalmanFilterDelegate(KeyValuePair<string, double> parameterValue);
    }
}
