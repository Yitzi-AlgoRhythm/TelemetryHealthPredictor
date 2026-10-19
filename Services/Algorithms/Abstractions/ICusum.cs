using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Model.AlgorithmResultObjects;

namespace TelemetryHealthPredictor.Services.Algorithms.Abstractions
{
    public interface ICusum
    {
        public CusumResult CusumDelegate(KalmanFilterResult kalmanFilterResult);
    }
}
