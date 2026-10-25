using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Data;

namespace TelemetryHealthPredictor.Algorithm.Delegates.Abstractions
{
    public interface IMahalanobis
    {
        public MahalanobisResult MahalanobisDelegate(KalmanFilterResult kalmanFilterResult);
    }
}
