using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;
using TelemetryHealthPredictor.Model.AlgorithmResultObjects;
using TelemetryHealthPredictor.Services.Algorithms.Abstractions;

namespace TelemetryHealthPredictor.Services.Algorithms.Implementations
{
    public class Mahalanobis : IMahalanobis
    {
        private readonly MahalanobisData _mahalanobisData;

        public Mahalanobis(MahalanobisData mahalanobisData)
        {
            _mahalanobisData = mahalanobisData;
        }

        public MahalanobisResult MahalanobisDelegate(KalmanFilterResult kalmanFilterResult)
        {
            return _mahalanobisData.RunMahalanobis(kalmanFilterResult);
        }
    }
}
