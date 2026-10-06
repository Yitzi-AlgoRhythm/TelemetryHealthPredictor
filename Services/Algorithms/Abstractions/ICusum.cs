using TelemetryHealthPredictor.Model.AlgorithmResultObjects;

namespace TelemetryHealthPredictor.Services.Algorithms.Abstractions
{
    public interface ICusum
    {
        public Dictionary<string, double> CusumDelegate(FilteredResult filteredResult);
    }
}
