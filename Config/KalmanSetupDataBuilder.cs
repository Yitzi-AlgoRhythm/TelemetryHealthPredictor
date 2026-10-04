using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.Options;

namespace TelemetryHealthPredictor.Config
{
    public static class KalmanSetupDataBuilder
    {
        public static KalmanSetupData Build(IServiceProvider provider)
        {
            KalmanSettings settings = provider.GetRequiredService<IOptions<KalmanSettings>>().Value;

            string[] measuredNames = Load(nameof(settings.MeasuredVariablesPath),
                settings.MeasuredVariablesPath, p => CSVReader.ReadStrings(p));
            string[] stateNames = Load(nameof(settings.StateVariablesPath),
                settings.StateVariablesPath, p => CSVReader.ReadStrings(p));

            Vector<double> x0 = Load(nameof(settings.X0Path),
                settings.X0Path, p => CSVReader.ReadVector(p));
            Matrix<double> p0 = Load(nameof(settings.P0Path),
                settings.P0Path, p => CSVReader.ReadMatrix(p));
            Vector<double> q0 = Load(nameof(settings.q0VectorPath),
                settings.q0VectorPath, p => CSVReader.ReadVector(p));
            Matrix<double> bigQ0 = Load(nameof(settings.Q0MatrixPath),
                settings.Q0MatrixPath, p => CSVReader.ReadMatrix(p));
            Vector<double> r0 = Load(nameof(settings.r0VectorPath),
                settings.r0VectorPath, p => CSVReader.ReadVector(p));
            Matrix<double> bigR0 = Load(nameof(settings.RMatrixPath),
                settings.RMatrixPath, p => CSVReader.ReadMatrix(p));
            Matrix<double> f = Load(nameof(settings.FPath),
                settings.FPath, p => CSVReader.ReadMatrix(p));
            Matrix<double> h = Load(nameof(settings.HPath),
                settings.HPath, p => CSVReader.ReadMatrix(p));
            Matrix<double> i = Load(nameof(settings.IdentityPath),
                settings.IdentityPath, p => CSVReader.ReadMatrix(p));

            int n = stateNames.Length;
            int m = measuredNames.Length;

            if (n == 0)
                throw new InvalidOperationException($"{nameof(settings.StateVariablesPath)}: no state variable names were read.");
            if (m == 0)
                throw new InvalidOperationException($"{nameof(settings.MeasuredVariablesPath)}: no measured variable names were read.");

            CheckVector(x0, n, nameof(settings.X0Path));
            CheckMatrix(p0, n, n, nameof(settings.P0Path));
            CheckVector(q0, n, nameof(settings.q0VectorPath));
            CheckMatrix(bigQ0, n, n, nameof(settings.Q0MatrixPath));
            CheckVector(r0, m, nameof(settings.r0VectorPath));
            CheckMatrix(bigR0, m, m, nameof(settings.RMatrixPath));
            CheckMatrix(f, n, n, nameof(settings.FPath));
            CheckMatrix(h, m, n, nameof(settings.HPath));

            double b = settings.ForgettingFactor;
            if (double.IsNaN(b) || b <= 0.0 || b > 1.0)
                throw new InvalidOperationException(
                    $"{nameof(settings.ForgettingFactor)} must be in the range (0, 1), but was {b}.");

            return new KalmanSetupData
            {
                MeasuredVariableNames = measuredNames,
                StateVariableNames = stateNames,
                InitValues_X0 = x0,
                InitErrorCovariance_P0 = p0,
                InitProcessBias_q0 = q0,
                InitMeasurementBias_r0 = r0,
                InitProcessNoiseCov_Q0 = bigQ0,
                InitMeasurementNoiseCov_R0 = bigR0,
                TransitionPrediction_F = f,
                StateToMeasurement_H = h,
                IdentityMatrix_I = i,
                ForgettingFactor_b = b
            };
        }

        private static T Load<T>(string settingName, string configuredPath, Func<string, T> reader)
        {
            if (!File.Exists(configuredPath))
                throw new FileNotFoundException($"{settingName}: file not found at '{configuredPath}'.", configuredPath);

            return reader(configuredPath);
        }

        private static void CheckVector(Vector<double> v, int expectedLength, string settingName)
        {
            if (v.Count != expectedLength)
            {
                throw new InvalidOperationException
                    ($"{settingName}: expected a vector of length {expectedLength} but got {v.Count}.");
            }
            if (!v.Enumerate().All(double.IsFinite))
            {
                throw new InvalidOperationException
                    ($"{settingName}: contains NaN or infinite values.");
            }
        }

        private static void CheckMatrix(Matrix<double> mat, int expectedRows, int expectedCols, string settingName)
        {
            if (mat.RowCount != expectedRows || mat.ColumnCount != expectedCols)
            {
                throw new InvalidOperationException
                    ($"{settingName}: expected a {expectedRows}x{expectedCols} matrix but got {mat.RowCount}x{mat.ColumnCount}.");
            }
            if (!mat.Enumerate().All(double.IsFinite))
            {
                throw new InvalidOperationException($"{settingName}: contains NaN or infinite values.");
            }
        }
    }
}
