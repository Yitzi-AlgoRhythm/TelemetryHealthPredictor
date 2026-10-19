using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Model.AlgorithmResultObjects;

namespace TelemetryHealthPredictor.Model.AlgorithmDataObjects
{
    public class CusumData
    {
        private readonly NamingData _namingData;

        public required double[] Slacks { get; init; }
        public required double[] Thresholds { get; init; }

        public required double[] PositiveCusums { get; set; }
        public required double[] NegativeCusums { get; set; }

        public required double[] Means { get; set; }
        public required double[] StandardDeviations { get; set; }

        public CusumData(NamingData namingData)
        {
            _namingData = namingData;
        }

        public CusumResult RunCusums(KalmanFilterResult kalmanFilterResult)
        {
            Vector<double> filteredParameterValues = kalmanFilterResult.FilteredParameterValues;

            List<int> anomalousIndices = [];

            for (int i = 0; i < filteredParameterValues.Count; i++)
            {
                double z = StandardDeviations[i] > 0
                    ? (filteredParameterValues[i] - Means[i]) / StandardDeviations[i]
                    : 0.0;

                PositiveCusums[i] = Math.Max(0.0, PositiveCusums[i] + z - Slacks[i]);
                NegativeCusums[i] = Math.Max(0.0, NegativeCusums[i] - z - Slacks[i]);

                if (PositiveCusums[i] > Thresholds[i] || NegativeCusums[i] > Thresholds[i])
                {
                    anomalousIndices.Add(i);
                }
            }

            Dictionary<string, double> result = [];

            foreach (int index in anomalousIndices)
            {
                result.Add(_namingData.ParameterNames[index], filteredParameterValues[index]);
            }

            return new CusumResult(result);
        }
    }
}
