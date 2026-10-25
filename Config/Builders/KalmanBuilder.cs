using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.Options;
using TelemetryHealthPredictor.Algorithm.Delegates.Implementations;
using TelemetryHealthPredictor.Config.Settings;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;

namespace TelemetryHealthPredictor.Config.Builders
{
    public static class KalmanBuilder
    {
        public static KalmanFilter Build(IServiceProvider provider)
        {
            KalmanSettings settings = provider.GetRequiredService<IOptions<KalmanSettings>>().Value;

            NamingData namingData = provider.GetRequiredService<NamingData>();

            Matrix<double> aMatrix = CSVReader.Load(nameof(settings.AMatrixPath),
                settings.AMatrixPath, p => CSVReader.ReadMatrix(p));

            Matrix<double> gMatrix = CSVReader.Load(nameof(settings.GMatrixPath),
                settings.GMatrixPath, p => CSVReader.ReadMatrix(p));

            Matrix<double> r0Matrix = CSVReader.Load(nameof(settings.R0MatrixPath),
                settings.R0MatrixPath, p => CSVReader.ReadMatrix(p));

            Matrix<double> p0Matrix = CSVReader.Load(nameof(settings.P0MatrixPath),
                settings.P0MatrixPath, p => CSVReader.ReadMatrix(p));

            Vector<double> qcVector = CSVReader.Load(nameof(settings.QcVectorPath),
                settings.QcVectorPath, p => CSVReader.ReadVector(p));

            Vector<double> x0Vector = CSVReader.Load(nameof(settings.X0VectorPath),
                settings.X0VectorPath, p => CSVReader.ReadVector(p));



            return new KalmanFilter
            (
                namingData,
                aMatrix,
                gMatrix,
                qcVector,
                r0Matrix,
                p0Matrix,
                x0Vector,
                settings.ForgettingFactor
            );
        }
    }
}
