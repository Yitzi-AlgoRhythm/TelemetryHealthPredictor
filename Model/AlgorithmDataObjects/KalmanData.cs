using MathNet.Numerics.LinearAlgebra;

namespace TelemetryHealthPredictor.Model.AlgorithmDataObjects
{
    public class KalmanData
    {
        public required Dictionary<string, int> MeasuredVariableNames { get; init; }

        private int _measuredVariableCount;
        private int _currentMeasuredVariableCount;
        private int[] _variableTimesMeasured;

        public required string[] StateVariableNames { get; init; }
        public required Matrix<double> TransitionMatrix { get; init; }
        public required Matrix<double> StateToMeasShapeMatrix { get; init; }
        public required Matrix<double> IdentityMatrix { get; init; }

        public Vector<double> PreMeasStatePredictionVector { get; set; } = null!;
        public required Vector<double> PostMeasStatePredictionVector { get; set; }
        public required Vector<double> ProcessBiasVector { get; set; }
        public Matrix<double> PreMeasStateUncertaintyMatrix { get; set; } = null!;
        public required Matrix<double> PostMeasStateUncertaintyMatrix { get; set; }
        public Matrix<double> PreviousUncertaintyMatrix { get; set; } = null!;
        public required Matrix<double> ProcessNoiseMatrix { get; set; }
        public required Vector<double> MeasBiasVector { get; set; }
        public Vector<double> Innovation { get; set; } = null!;
        public Vector<double> Measurements { get; set; } = null!;
        public required Matrix<double> MeasNoiseMatrix { get; init; }
        public Matrix<double> InnovationUncertaintyMatrixInversed { get; set; } = null!;
        public Matrix<double> KalmanGain { get; set; } = null!;
        public required double ForgettingFactor { get; init; }

        private int _step;

        private double _sageHusaWeightFactor;

        public KalmanData(int measuredVariableCount)
        {
            _variableTimesMeasured = new int[measuredVariableCount];
            _measuredVariableCount = measuredVariableCount;
            Measurements = Vector<double>.Build.Dense(measuredVariableCount);
            _currentMeasuredVariableCount = 0;
            _step = 1;
        }

        public Vector<double> RunKalmanFilter()
        {
            Predict();
            CalculateInnovation();
            PredictInnovationCovariance();
            CalculateKalmanGain();
            Update();
            SageHusaCalibration();

            _step++;

            return PostMeasStatePredictionVector.Clone();
        }

        public bool AllMeasurementsIn(KeyValuePair<string, double> parameterValue)
        {
            int paramIndex = MeasuredVariableNames[parameterValue.Key];

            Measurements[paramIndex] = parameterValue.Value;

            _variableTimesMeasured[paramIndex]++;

            if (_variableTimesMeasured[paramIndex] == 1)
            {
                _currentMeasuredVariableCount++;

                if (_currentMeasuredVariableCount == _measuredVariableCount)
                {
                    _currentMeasuredVariableCount = 0;
                    Array.Fill(_variableTimesMeasured, 0);

                    return true;
                }
            }
            return false;
        }

        private void Predict()
        {
            PreMeasStatePredictionVector = TransitionMatrix * PostMeasStatePredictionVector + ProcessBiasVector;

            PreMeasStateUncertaintyMatrix = (TransitionMatrix * PostMeasStateUncertaintyMatrix).TransposeAndMultiply(TransitionMatrix) + ProcessNoiseMatrix;

            PreMeasStateUncertaintyMatrix = (PreMeasStateUncertaintyMatrix + PreMeasStateUncertaintyMatrix.Transpose()) * 0.5;
        }

        private void CalculateInnovation()
        {
            Innovation = Measurements - (StateToMeasShapeMatrix * PreMeasStatePredictionVector) - MeasBiasVector;
        }

        private void PredictInnovationCovariance()
        {
            InnovationUncertaintyMatrixInversed = ((StateToMeasShapeMatrix * PreMeasStateUncertaintyMatrix).TransposeAndMultiply(StateToMeasShapeMatrix) + MeasNoiseMatrix).Inverse();
        }

        private void CalculateKalmanGain()
        {
            KalmanGain = PreMeasStateUncertaintyMatrix.TransposeAndMultiply(StateToMeasShapeMatrix) * InnovationUncertaintyMatrixInversed;
        }

        private void Update()
        {
            PostMeasStatePredictionVector = PreMeasStatePredictionVector + KalmanGain * Innovation;

            PreviousUncertaintyMatrix = PostMeasStateUncertaintyMatrix;

            PostMeasStateUncertaintyMatrix = (IdentityMatrix - KalmanGain * StateToMeasShapeMatrix) * PreMeasStateUncertaintyMatrix;

            PostMeasStateUncertaintyMatrix = (PostMeasStateUncertaintyMatrix + PostMeasStateUncertaintyMatrix.Transpose()) * 0.5;
        }

        private void SageHusaCalibration()
        {
            _sageHusaWeightFactor = (1 - ForgettingFactor) / (1 - Math.Pow(ForgettingFactor, _step + 1));

            Matrix<double> noiseFreeUncertainty = (TransitionMatrix * PreviousUncertaintyMatrix).TransposeAndMultiply(TransitionMatrix);

            Matrix<double> correctionUncertainty = ((KalmanGain * Innovation).OuterProduct(Innovation)).TransposeAndMultiply(KalmanGain);

            Matrix<double> processNoiseContribution = _sageHusaWeightFactor * (correctionUncertainty + PostMeasStateUncertaintyMatrix - noiseFreeUncertainty);

            Matrix<double> newProcessNoiseMatrix = (1 - _sageHusaWeightFactor) * ProcessNoiseMatrix + processNoiseContribution;

            if (IsPositiveDefinite(newProcessNoiseMatrix))
            {
                ProcessNoiseMatrix = newProcessNoiseMatrix;

                ProcessNoiseMatrix = (ProcessNoiseMatrix + ProcessNoiseMatrix.Transpose()) * 0.5;
            }
        }

        private static bool IsPositiveDefinite(Matrix<double> m)
        {
            if (m.Diagonal().Minimum() <= 0) return false;

            try
            {
                m.Cholesky();
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
