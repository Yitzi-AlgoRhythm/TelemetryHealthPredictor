
using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Model.AlgorithmResultObjects;

namespace TelemetryHealthPredictor.Model.AlgorithmDataObjects
{
    public class MahalanobisData
    {
        private readonly NamingData _namingData;

        public required Vector<double> Means { get; init; }
        public required Matrix<double> CovarianceMatrixInversed { get; init; }

        public MahalanobisData(NamingData namingData)
        {
            _namingData = namingData;
        }

        public MahalanobisResult GetMahalanobisDistance(Vector<double> filteredResult)
        {
            Vector<double> differences = filteredResult - Means;

            double distanceSquared = (differences * CovarianceMatrixInversed) * differences;

            double mahalanobisDistance = Math.Sqrt(distanceSquared);

            return new MahalanobisResult()
            {
                DistanceScore = mahalanobisDistance,
                Anomalies = []
            };
        }
    }
}
