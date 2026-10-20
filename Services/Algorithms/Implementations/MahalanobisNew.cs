using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;
using TelemetryHealthPredictor.Model.AlgorithmResultObjects;
using TelemetryHealthPredictor.Services.Algorithms.Abstractions;

namespace TelemetryHealthPredictor.Services.Algorithms.Implementations
{
    public class MahalanobisNew : IMahalanobis
    {
        private readonly double _stepSize;
        private readonly int _maxInnerIterations;

        private readonly string[] _parameterNames;
        private readonly int _parameterCount;

        private readonly Matrix<double> _correlationMatrixInversed;

        private readonly double _distanceSquaredThreshold;


        public MahalanobisNew(double stepSize, int maxInnerIterations, NamingData namingData, Matrix<double> covarianceMatrix, double distanceThreshold)
        {
            _stepSize = stepSize;
            _maxInnerIterations = maxInnerIterations;

            _parameterNames = namingData.ParameterNames;
            _parameterCount = namingData.ParameterNames.Length;

            _correlationMatrixInversed = CovarianceToCorrelationInversed(covarianceMatrix);

            _distanceSquaredThreshold = distanceThreshold;
        }

        public MahalanobisResult MahalanobisDelegate(KalmanFilterResult kalmanFilterResult)
        {
            Vector<double> zScores = kalmanFilterResult.ParameterZScores;
            // Z-scores are the standardized values of the parameters (mean 0, std 1)

            double distanceSquared = GetDistanceSquared(zScores);

            Dictionary<string, double> outliers = [];

            if (distanceSquared > _distanceSquaredThreshold)
            {
                outliers = FindOutlierParameters(zScores, distanceSquared);
            }

            return new MahalanobisResult()
            {
                DistanceScore = Math.Sqrt(distanceSquared),
                OutlierParameters = outliers
            };
        }

        private double GetDistanceSquared(Vector<double> zScores)
        {
            double distanceSquared = zScores.DotProduct(_correlationMatrixInversed * zScores);

            return distanceSquared;
        }

        private Dictionary<string, double> FindOutlierParameters(Vector<double> zScores, double distanceSquared)
        {
            Vector<double> originalPhi = GetShapleyContributions(zScores);

            Vector<double> xTilde = zScores.Clone();
            HashSet<int> flagged = [];
            double currentDistanceSquared = distanceSquared;

            while (currentDistanceSquared > _distanceSquaredThreshold && flagged.Count < _parameterCount)
            {
                Vector<double> phi = GetShapleyContributions(xTilde);

                int next = -1;
                double best = double.NegativeInfinity;
                for (int i = 0; i < _parameterCount; i++)
                {
                    if (!flagged.Contains(i) && phi[i] > best)
                    {
                        best = phi[i];
                        next = i;
                    }
                }
                flagged.Add(next);

                for (int iterations = 0; iterations < _maxInnerIterations; iterations++)
                {
                    phi = GetShapleyContributions(xTilde);

                    double flaggedMax = double.NegativeInfinity;
                    double unflaggedMax = double.NegativeInfinity;
                    for (int i = 0; i < _parameterCount; i++)
                    {
                        if (flagged.Contains(i))
                        {
                            flaggedMax = Math.Max(flaggedMax, phi[i]);
                        }
                        else
                        {
                            unflaggedMax = Math.Max(unflaggedMax, phi[i]);
                        }
                    }

                    if (flaggedMax <= unflaggedMax) break;

                    if (flagged.Count == _parameterCount &&
                        GetDistanceSquared(xTilde) <= _distanceSquaredThreshold) break;

                    foreach (int j in flagged)
                        xTilde[j] -= xTilde[j] * _stepSize;
                }

                currentDistanceSquared = GetDistanceSquared(xTilde);
            }

            return flagged.ToDictionary(i => _parameterNames[i], i => originalPhi[i]);
        }

        // φ = (x − μ) ∘ Ω(x − μ), with μ = 0 because the inputs are z-scores
        private Vector<double> GetShapleyContributions(Vector<double> zScores)
        {
            return zScores.PointwiseMultiply(_correlationMatrixInversed * zScores);
        }

        private static Matrix<double> CovarianceToCorrelationInversed(Matrix<double> covariance)
        {
            Vector<double> standardDeviations = covariance.Diagonal().Map(Math.Sqrt);
            return Matrix<double>.Build.Dense(covariance.RowCount, covariance.ColumnCount,
                (i, j) => covariance[i, j] / (standardDeviations[i] * standardDeviations[j])).Inverse();
        }
    }
}
