using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.Options;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;

namespace TelemetryHealthPredictor.Config
{
    public static class KalmanDataBuilder
    {
        public static KalmanData Build(IServiceProvider provider)
        {
            KalmanSettings settings = provider.GetRequiredService<IOptions<KalmanSettings>>().Value;

            string[] measuredNames = CSVReader.Load(nameof(settings.MeasuredVariablesPath),
                settings.MeasuredVariablesPath, p => CSVReader.ReadStrings(p));

            string[] stateNames = CSVReader.Load(nameof(settings.StateVariablesPath),
                settings.StateVariablesPath, p => CSVReader.ReadStrings(p));

            Vector<double> x0 = CSVReader.Load(nameof(settings.X0Path),
                settings.X0Path, p => CSVReader.ReadVector(p));

            Matrix<double> p0 = CSVReader.Load(nameof(settings.P0Path),
                settings.P0Path, p => CSVReader.ReadMatrix(p));

            Vector<double> q0 = CSVReader.Load(nameof(settings.q0VectorPath),
                settings.q0VectorPath, p => CSVReader.ReadVector(p));

            Matrix<double> bigQ0 = CSVReader.Load(nameof(settings.Q0MatrixPath),
                settings.Q0MatrixPath, p => CSVReader.ReadMatrix(p));

            Vector<double> r0 = CSVReader.Load(nameof(settings.r0VectorPath),
                settings.r0VectorPath, p => CSVReader.ReadVector(p));

            Matrix<double> bigR0 = CSVReader.Load(nameof(settings.RMatrixPath),
                settings.RMatrixPath, p => CSVReader.ReadMatrix(p));

            Matrix<double> f = CSVReader.Load(nameof(settings.FPath),
                settings.FPath, p => CSVReader.ReadMatrix(p));

            Matrix<double> h = CSVReader.Load(nameof(settings.HPath),
                settings.HPath, p => CSVReader.ReadMatrix(p));

            Matrix<double> i = CSVReader.Load(nameof(settings.IdentityPath),
                settings.IdentityPath, p => CSVReader.ReadMatrix(p));

            double b = settings.ForgettingFactor;

            Dictionary<string, int> measuredVariablesDict = new Dictionary<string, int>();
            for (int index = 0; index < measuredNames.Length; index++)
            {
                measuredVariablesDict[measuredNames[index]] = index;
            }



            return new KalmanData(measuredNames.Length)
            {
                MeasuredVariableNames = measuredVariablesDict,
                StateVariableNames = stateNames,
                TransitionMatrix = f,
                StateToMeasShapeMatrix = h,
                IdentityMatrix = i,
                PostMeasStatePredictionVector = x0,
                ProcessBiasVector = q0,
                PostMeasStateUncertaintyMatrix = p0,
                ProcessNoiseMatrix = bigQ0,
                MeasBiasVector = r0,
                MeasNoiseMatrix = bigR0,
                ForgettingFactor = b,
                Means = Vector<double>.Build.Dense(stateNames.Length),
                StandardDeviations = Vector<double>.Build.Dense(stateNames.Length)
            };
        }
    }
}
