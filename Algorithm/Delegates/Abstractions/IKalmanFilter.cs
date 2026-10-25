using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Data;

namespace TelemetryHealthPredictor.Algorithm.Delegates.Abstractions
{
    public interface IKalmanFilter
    {
        public KalmanFilterResult[] KalmanFilterDelegate(IncomingFlightParameter[] parameters);
    }
}
