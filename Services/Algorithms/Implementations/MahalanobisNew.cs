using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;
using TelemetryHealthPredictor.Model.AlgorithmResultObjects;
using TelemetryHealthPredictor.Services.Algorithms.Abstractions;

namespace TelemetryHealthPredictor.Services.Algorithms.Implementations
{
    public class MahalanobisNew : IMahalanobis
    {
        private readonly string[] _parameterNames;

        private readonly Matrix<double> _correlationMatrix;


        public MahalanobisNew(NamingData namingData, Matrix<double> covarianceMatrix)
        {
            _parameterNames = namingData.ParameterNames;

            _correlationMatrix = CovarianceToCorrelation(covarianceMatrix);
        }

        public MahalanobisResult MahalanobisDelegate(KalmanFilterResult kalmanFilterResult)
        {
            Vector<double> zScores = kalmanFilterResult.ParameterZScores;

            // Solve R * x = z instead of explicitly inverting R.
            // Cholesky is fast and stable for symmetric positive-definite matrices.
            Vector<double> x;
            try
            {
                x = _correlationMatrix.Cholesky().Solve(zScores);
            }
            catch (ArgumentException)
            {
                // Not positive definite (e.g. perfectly collinear variables):
                // fall back to the pseudo-inverse via SVD.
                x = _correlationMatrix.PseudoInverse() * zScores;
            }

            double d2 = zScores.DotProduct(x);

            return new MahalanobisResult()
            {
                DistanceScore = Math.Sqrt(d2),
                Anomalies = []
            };
        }

        private static Matrix<double> CovarianceToCorrelation(Matrix<double> covariance)
        {
            Vector<double> standardDeviations = covariance.Diagonal().Map(Math.Sqrt);
            return Matrix<double>.Build.Dense(covariance.RowCount, covariance.ColumnCount,
                (i, j) => covariance[i, j] / (standardDeviations[i] * standardDeviations[j]));
        }
    }
}
