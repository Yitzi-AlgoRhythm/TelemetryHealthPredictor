using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Model.AlgorithmResultObjects;
using TelemetryHealthPredictor.Services.Algorithms.Abstractions;

namespace TelemetryHealthPredictor.Services.Algorithms.Mockups
{
    public class MahalanobisMockup : IMahalanobis
    {
        public MahalanobisResult MahalanobisDelegate(Vector<double> filteredResult)
        {
            Console.WriteLine("Calculating Mahalanobis Distance");

            return new MahalanobisResult()
            {
                DistanceScore = 0,
                Anomalies = []
            };
        }
    }
}
