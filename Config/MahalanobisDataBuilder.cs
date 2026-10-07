using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.Options;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;

namespace TelemetryHealthPredictor.Config
{
    public class MahalanobisDataBuilder
    {
        public static MahalanobisData Build(IServiceProvider provider)
        {
            MahalanobisSettings settings = provider.GetRequiredService<IOptions<MahalanobisSettings>>().Value;

            Matrix<double> covarianceMatrix = CSVReader.Load(nameof(settings.CovarianceMatrixPath),
                settings.CovarianceMatrixPath, p => CSVReader.ReadMatrix(p));

            Vector<double> means = CSVReader.Load(nameof(settings.MeansPath),
                settings.MeansPath, p => CSVReader.ReadVector(p));

            return new MahalanobisData()
            {
                CovarianceMatrixInversed = covarianceMatrix.Inverse(),
                Means = means
            };
        }
    }
}
