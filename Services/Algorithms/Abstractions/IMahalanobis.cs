using TelemetryHealthPredictor.Model.AlgorithmResultObjects;

namespace TelemetryHealthPredictor.Services.Algorithms.Abstractions
{
    public interface IMahalanobis
    {
        public MahalanobisResult MahalanobisDistance(FilteredResult filteredResult);
    }
}
