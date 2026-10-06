
using MathNet.Numerics.LinearAlgebra;

namespace TelemetryHealthPredictor.Model.AlgorithmDataObjects
{
    public class MahalanobisData
    {
        public double MahalanobisDistance { get; set; }
        public Vector<double> Values { get; set; } = null!;
        public Vector<double> Means { get; set; } = null!;
        public Matrix<double> CovarianceMatrix { get; set; } = null!;
    }
}
