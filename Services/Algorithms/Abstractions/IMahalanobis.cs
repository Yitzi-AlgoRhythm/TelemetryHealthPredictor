using TelemetryHealthPredictor.Model.AlgorithmResultObjects;

namespace TelemetryHealthPredictor.Services.Algorithms.Abstractions
{
    public interface IMahalanobis
    {
        public MahalanobisResult MahalanobisDelegate(FilteredResult filteredResult);
    }
}
