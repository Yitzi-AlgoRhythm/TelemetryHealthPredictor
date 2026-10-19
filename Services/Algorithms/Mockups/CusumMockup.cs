using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Model.AlgorithmResultObjects;
using TelemetryHealthPredictor.Services.Algorithms.Abstractions;

namespace TelemetryHealthPredictor.Services.Algorithms.Mockups
{
    public class CusumMockup : ICusum
    {
        public CusumResult CusumDelegate(KalmanFilterResult kalmanFilterResult)
        {
            Console.WriteLine("Calculating CUSUMs");

            return new CusumResult([]);
        }
    }
}
