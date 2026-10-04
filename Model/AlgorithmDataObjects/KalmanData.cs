using MathNet.Numerics.LinearAlgebra;

namespace TelemetryHealthPredictor.Model.AlgorithmDataObjects
{
    public class KalmanData
    {
        public required Matrix<double> TransitionMatrix { get; init; }
        public required Matrix<double> StateToMeasShapeMatrix { get; init; }
        public required Matrix<double> IdentityMatrix { get; init; }

        public required Vector<double> PreMeasStatePredictionVector { get; set; }
        public required Vector<double> PostMeasStatePredictionVector { get; set; }
        public required Vector<double> ProcessBiasVector { get; set; }
        public required Matrix<double> PreMeasStateUncertaintyMatrix { get; set; }
        public required Matrix<double> PostMeasStateUncertaintyMatrix { get; set; }
        public required Matrix<double> ProcessNoiseMatrix { get; set; }
        public required Vector<double> MeasBiasVector { get; set; }
        public required Vector<double> Innovation { get; set; }
        public required Vector<double> Measurements { get; set; }
        public required Matrix<double> MeasNoiseMatrix { get; set; }
        public required Matrix<double> InnovationUncertaintyMatrix { get; set; }
        public required Matrix<double> KalmanGain { get; set; }
        public required double SageHusaWeightFactor { get; set; }
    }
}
