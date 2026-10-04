using MathNet.Numerics.LinearAlgebra;

namespace TelemetryHealthPredictor.Model.AlgorithmResultObjects
{
    public class FilteredResult
    {
        public required Vector<double> FilteredValues { get; init; }
        public required Matrix<double> CovarianceMatrix { get; init; }
        public required Vector<double> Means { get; init; }
        public required Vector<double> StandardDeviations { get; init; }
    }
}
