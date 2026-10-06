using TelemetryHealthPredictor.Model.AlgorithmResultObjects;
using TelemetryHealthPredictor.Services.Algorithms.Abstractions;

namespace TelemetryHealthPredictor.Services.Algorithms.Mockups
{
    public class CusumMockup : ICusum
    {
        public Dictionary<string, double> CusumDelegate(FilteredResult filteredResult)
        {
            Console.WriteLine("Calculating CUSUMs");

            return [];
        }
    }
}
