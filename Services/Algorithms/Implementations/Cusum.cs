using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;
using TelemetryHealthPredictor.Model.AlgorithmResultObjects;
using TelemetryHealthPredictor.Services.Algorithms.Abstractions;

namespace TelemetryHealthPredictor.Services.Algorithms.Implementations
{
    public class Cusum : ICusum
    {
        private readonly CusumData _cusumData;

        public Cusum(CusumData cusumData)
        {
            _cusumData = cusumData;
        }

        public CusumResult CusumDelegate(KalmanFilterResult kalmanFilterResult)
        {
            return _cusumData.RunCusums(kalmanFilterResult);
        }
    }
}
