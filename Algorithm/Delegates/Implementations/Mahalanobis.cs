using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Algorithm.Delegates.Abstractions;
using TelemetryHealthPredictor.Data;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;

namespace TelemetryHealthPredictor.Algorithm.Delegates.Implementations
{
    public class Mahalanobis : IMahalanobis
    {
        private readonly double _stepSize;

        private readonly string[] _parameterNames;
        private readonly int _parameterCount;

        private readonly Matrix<double> _correlationMatrixInversed;

        private readonly double _distanceSquaredThreshold;


        /// <param name="stepSize">
        /// Represents how far the value of a z-score should be moved towards its mean
        /// in each iteration of finding outlier parameters.
        /// </param>
        /// <param name="namingData">NamingData object with an array of parameter names.</param>
        /// <param name="covarianceMatrix">Represents the covariance of the parameters' Z-scores.</param>
        /// <param name="distanceSquaredThreshold">A set of Z-scores will be considered anomalous if MD^2 crosses this threshold.</param>
        public Mahalanobis
        (
            double stepSize, 
            NamingData namingData, 
            Matrix<double> covarianceMatrix, 
            double distanceSquaredThreshold
        )
        {
            _stepSize = stepSize;

            _parameterNames = namingData.ParameterNames;
            _parameterCount = namingData.ParameterNames.Length;

            _correlationMatrixInversed = CovarianceToCorrelationInversed(covarianceMatrix);

            _distanceSquaredThreshold = distanceSquaredThreshold;
        }

        public MahalanobisResult MahalanobisDelegate(KalmanFilterResult kalmanFilterResult)
        {
            Vector<double> zScores = kalmanFilterResult.ParameterZScores;
            // Z-scores are the standardized values of the parameters (mean 0, std 1)

            double distanceSquared = GetDistanceSquared(zScores);

            Dictionary<string, double> outliers = [];

            if (distanceSquared > _distanceSquaredThreshold)
            {
                outliers = FindOutlierParameters(zScores, distanceSquared, kalmanFilterResult.ParameterValues);
            }

            return new MahalanobisResult()
            {
                DistanceSquared = distanceSquared,
                OutlierParameters = outliers,
                DeltaT = kalmanFilterResult.DeltaT
            };
        }

        private double GetDistanceSquared(Vector<double> zScores)
        {
            double distanceSquared = zScores.DotProduct(_correlationMatrixInversed * zScores);

            return distanceSquared;
        }

        private Dictionary<string, double> FindOutlierParameters(Vector<double> zScores, double distanceSquared, double[] parameterValues)
        {
            Vector<double> xTilde = zScores.Clone();
            HashSet<int> flagged = [];
            double currentDistanceSquared = distanceSquared;

            double flaggedMax;
            double unflaggedMax;

            while (currentDistanceSquared > _distanceSquaredThreshold && flagged.Count < _parameterCount)
            {
                Vector<double> phi = GetShapleyContributions(xTilde);

                int nextFlaggedIndex = -1;
                double maxContribution = double.NegativeInfinity;
                for (int i = 0; i < _parameterCount; i++)
                {
                    if (!flagged.Contains(i) && phi[i] > maxContribution)
                    {
                        maxContribution = phi[i];
                        nextFlaggedIndex = i;
                    }
                }

                flagged.Add(nextFlaggedIndex);

                do
                {
                    phi = GetShapleyContributions(xTilde);

                    flaggedMax = double.NegativeInfinity;
                    unflaggedMax = double.NegativeInfinity;
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

                    foreach (int flaggedIndex in flagged)
                        xTilde[flaggedIndex] -= xTilde[flaggedIndex] * _stepSize;
                }
                while (flaggedMax <= unflaggedMax);

                currentDistanceSquared = GetDistanceSquared(xTilde);
            }

            return flagged.ToDictionary(i => _parameterNames[i], i => parameterValues[i]);
        }

        /// <summary> 
        /// Calculates the vector φ = (x − μ) ∘ Ω(x − μ), with μ = 0 because the inputs are z-scores.
        /// The resulting vector represents the individual contribution of each parameter's z-score to the Mahalanobis Distance.
        /// </summary>
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
