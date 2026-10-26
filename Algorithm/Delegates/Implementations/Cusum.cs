using TelemetryHealthPredictor.Algorithm.Delegates.Abstractions;
using TelemetryHealthPredictor.Data;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;

namespace TelemetryHealthPredictor.Algorithm.Delegates.Implementations
{
    /*
     * The point of this class is to provide a delegate function for the CUSUM block.
     * The class will be initialized with zeroed arrays representing the initial positive & negative CUSUMs of all parameters.
     * Given that it'll be receiving standardized Z-scores, 
     * it should be initialized with consts for Means and Standard Deviations - 0 and 1, respectively.
     * It will also be initialized with arrays representing the slacks (K) and thresholds (h) of each parameter.
     * The function will recalculate the CUSUMs only of the parameters measured this time round,
     * then return the CUSUMs above their thresholds.
     */
    public class Cusum : ICusum
    {
        // Inputs are already standardized Z-scores, so the in-control mean is 0 and std dev is 1.
        private const double Mean = 0.0;
        private const double StdDev = 1.0;

        private readonly string[] _parameterNames;
        private readonly double[] _slacks;      // K per parameter
        private readonly double[] _thresholds;  // h per parameter

        private readonly double[] _positiveCusums;
        private readonly double[] _negativeCusums;

        /// <param name="namingData">NamingData object with an array of parameter names.</param>
        /// <param name="slacks">Slack (K) per parameter, in units of standard deviations.</param>
        /// <param name="thresholds">Decision threshold (h) per parameter, in units of standard deviations.</param>
        public Cusum(NamingData namingData, double[] slacks, double[] thresholds)
        {
            _parameterNames = namingData.ParameterNames;

            _slacks = (double[])slacks.Clone();
            _thresholds = (double[])thresholds.Clone();

            // Zero-initialised by default.
            _positiveCusums = new double[_parameterNames.Length];
            _negativeCusums = new double[_parameterNames.Length];
        }



        public CusumResult CusumDelegate(KalmanFilterResult kalmanFilterResult)
        {
            RecalculateCusums(kalmanFilterResult);

            return GetAnomalousCusums(kalmanFilterResult);
        }



        private void RecalculateCusums(KalmanFilterResult kalmanFilterResult)
        {
            foreach (int i in kalmanFilterResult.IndicesMeasured)
            {
                double zScore = kalmanFilterResult.ParameterZScores[i];

                double k = _slacks[i];

                // Recalculate positive & negative CUSUMs based on z-scores.
                _positiveCusums[i] = Math.Max(0.0, _positiveCusums[i] + zScore - k);
                _negativeCusums[i] = Math.Max(0.0, _negativeCusums[i] - zScore - k);
            }
        }

        private CusumResult GetAnomalousCusums(KalmanFilterResult kalmanFilterResult)
        {
            List<string> names = [];
            List<double> amountsAboveThresholds = [];
            List<double> values = [];

            foreach (int i in kalmanFilterResult.IndicesMeasured)
            {
                double h = _thresholds[i];
                double positiveCusum = _positiveCusums[i];
                double negativeCusum = _negativeCusums[i];

                bool posExceeded = positiveCusum > h;
                bool negExceeded = negativeCusum > h;

                if (posExceeded || negExceeded)
                {
                    double cusum = (posExceeded ? positiveCusum : -negativeCusum);

                    amountsAboveThresholds.Add(cusum / h - 1);

                    names.Add(_parameterNames[i]);

                    values.Add(kalmanFilterResult.ParameterValues[i]);
                }
            }

            return new CusumResult()
            {
                AnomalousParameterNames = names,
                AmountsAboveThresholds = amountsAboveThresholds,
                AnomalousParameterValues = values
            };
        }
    }
}