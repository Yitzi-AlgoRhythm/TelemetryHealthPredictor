using MathNet.Numerics.LinearAlgebra;

namespace TelemetryHealthPredictor.Model.AlgorithmDataObjects
{
    public class KalmanData
    {
        public required string[] MeasuredVariableNames { get; init; }
        public required string[] StateVariableNames { get; init; }
        public required Matrix<double> TransitionMatrix { get; init; }
        public required Matrix<double> StateToMeasShapeMatrix { get; init; }
        public required Matrix<double> IdentityMatrix { get; init; }

        public Vector<double> PreMeasStatePredictionVector { get; set; } = null!;
        public required Vector<double> PostMeasStatePredictionVector { get; set; }
        public required Vector<double> ProcessBiasVector { get; set; }
        public Matrix<double> PreMeasStateUncertaintyMatrix { get; set; } = null!;
        public required Matrix<double> PostMeasStateUncertaintyMatrix { get; set; }
        public required Matrix<double> ProcessNoiseMatrix { get; set; }
        public required Vector<double> MeasBiasVector { get; set; }
        public Vector<double> Innovation { get; set; } = null!;
        public Vector<double> Measurements { get; set; } = null!;
        public required Matrix<double> MeasNoiseMatrix { get; set; }
        public Matrix<double> InnovationUncertaintyMatrix { get; set; } = null!;
        public Matrix<double> KalmanGain { get; set; } = null!;
        public double SageHusaWeightFactor { get; set; }
    }
}
