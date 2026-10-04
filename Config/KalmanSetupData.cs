using MathNet.Numerics.LinearAlgebra;

namespace TelemetryHealthPredictor.Config
{
    public class KalmanSetupData
    {
        public required string[] MeasuredVariableNames { get; init; }
        public required string[] StateVariableNames { get; init; }
        public required Vector<double> InitValues_X0 { get; init; }
        public required Matrix<double> InitErrorCovariance_P0 { get; init; }
        public required Vector<double> InitProcessBias_q0 { get; init; }
        public required Vector<double> InitMeasurementBias_r0 { get; init; }
        public required Matrix<double> InitProcessNoiseCov_Q0 { get; init; }
        public required Matrix<double> InitMeasurementNoiseCov_R0 { get; init; }
        public required Matrix<double> TransitionPrediction_F {  get; init; }
        public required Matrix<double> StateToMeasurement_H { get; init; }
        public required double ForgettingFactor_b { get; init; }
    }
}
