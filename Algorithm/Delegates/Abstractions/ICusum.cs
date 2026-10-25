using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Data;

namespace TelemetryHealthPredictor.Algorithm.Delegates.Abstractions
{
    public interface ICusum
    {
        public CusumResult CusumDelegate(KalmanFilterResult kalmanFilterResult);
    }
}
