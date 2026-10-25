using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.Options;
using TelemetryHealthPredictor.Algorithm.Delegates.Implementations;
using TelemetryHealthPredictor.Config.Settings;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;

namespace TelemetryHealthPredictor.Config.Builders
{
    public class MahalanobisBuilder
    {
        public static Mahalanobis Build(IServiceProvider provider)
        {
            MahalanobisSettings settings = provider.GetRequiredService<IOptions<MahalanobisSettings>>().Value;

            NamingData namingData = provider.GetRequiredService<NamingData>();

            double stepSize = settings.StepSize;
            double distanceSquaredThreshold = settings.DistanceSquaredThreshold;

            Matrix<double> covarianceMatrix = CSVReader.Load(nameof(settings.CovarianceMatrixPath),
                settings.CovarianceMatrixPath, p => CSVReader.ReadMatrix(p));

            return new Mahalanobis
            (
                stepSize,
                namingData,
                covarianceMatrix,
                distanceSquaredThreshold
            );
        }
    }
}
