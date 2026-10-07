using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Model.AlgorithmResultObjects;

namespace TelemetryHealthPredictor.Services.Algorithms.Abstractions
{
    public interface IMahalanobis
    {
        public MahalanobisResult MahalanobisDelegate(Vector<double> filteredResult);
    }
}
