using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;
using TelemetryHealthPredictor.Model.AlgorithmResultObjects;

namespace TelemetryHealthPredictor.Services.Algorithms.Abstractions
{
    public interface IKalmanFilter
    {
        public KalmanFilterResult[] KalmanFilterDelegate(IncomingFlightParameter[] parameters);
    }
}
