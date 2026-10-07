using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Services.Algorithms.Abstractions;

namespace TelemetryHealthPredictor.Services.Algorithms.Mockups
{
    public class CusumMockup : ICusum
    {
        public Dictionary<string, double> CusumDelegate(Vector<double> filteredResult)
        {
            Console.WriteLine("Calculating CUSUMs");

            return [];
        }
    }
}
