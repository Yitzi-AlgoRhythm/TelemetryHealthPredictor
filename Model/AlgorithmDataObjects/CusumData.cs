using MathNet.Numerics.LinearAlgebra;

namespace TelemetryHealthPredictor.Model.AlgorithmDataObjects
{
    public class CusumData
    {
        public required double[] Slacks { get; init; }
        public required double[] Thresholds { get; init; }

        public required double[] PositiveCusums { get; set; }
        public required double[] NegativeCusums { get; set; }

        public required double[] Means { get; set; }
        public required double[] StandardDeviations { get; set; }



        public Dictionary<string, double> GetAnomalousCusums(Vector<double> filteredResult)
        {
            List<int> anomalousIndices = [];

            for (int i = 0; i < filteredResult.Count; i++)
            {
                double z = StandardDeviations[i] > 0
                    ? (filteredResult[i] - Means[i]) / StandardDeviations[i]
                    : 0.0;

                PositiveCusums[i] = Math.Max(0.0, PositiveCusums[i] + z - Slacks[i]);
                NegativeCusums[i] = Math.Max(0.0, NegativeCusums[i] - z - Slacks[i]);

                if (PositiveCusums[i] > Thresholds[i] || NegativeCusums[i] > Thresholds[i])
                {
                    anomalousIndices.Add(i);
                }
            }

            return [];
        }
    }
}
