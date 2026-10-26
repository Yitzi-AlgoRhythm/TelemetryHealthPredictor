using TelemetryHealthPredictor.Algorithm.Delegates.Abstractions;
using TelemetryHealthPredictor.Algorithm.Delegates.Helpers;
using TelemetryHealthPredictor.Data;

namespace TelemetryHealthPredictor.Algorithm.Delegates.Implementations
{
    public class Fusion : IFusion
    {
        private const double MaxScore = 100.0;
        private const double MinScore = 1.0;

        private readonly AnchorLadder _cusumLadder;
        private readonly AnchorLadder _d2Ladder;

        private readonly double _d2Weight;
        private readonly double _tDecay;
        private readonly double _hysteresisEntryValue, _hysteresisExitValue;

        private int _healthScore;

        // Fields used to keep track of values throughout algorithm.
        private double _mahalanobisDistanceSquared;
        private double _deltaT;
        private double _cusumRaw;
        private double _standardizedMahalanobis;   // 1-100 score from the d² ladder
        private double _standardizedCusum;         // 1-100 score from the CUSUM ladder
        private double _preDecayHealthScore;
        private double _decayedHealthScore = MaxScore; // state carried between frames
        private Boolean _inAlert;

        /// <param name="cusumLevels">
        /// A list of levels for CUSUM Raw.
        /// Each one represents how often it will be at that value or higher.
        /// Index 0 - the value that is exceeded 100% of the time in the dataset.
        /// Index 1 - the value that is exceeded 10% of the time in the dataset, etc.
        /// </param>
        /// <param name="d2Levels">
        /// A list of levels for Mahalanobis Distance squared.
        /// Each one represents how often it will be at that value or higher.
        /// Index 0 - the value that is exceeded 100% of the time in the dataset.
        /// Index 1 - the value that is exceeded 10% of the time in the dataset, etc.
        /// Must be the same length as cusumLevels, with the same index meaning the same frequency.
        /// </param>
        /// <param name="d2Weight">
        /// How much weight is given to Mahalanobis Distance in the health score calculation.
        /// 1 - d2Weight = the weight given to CUSUM.
        /// </param>
        /// <param name="tDecay">
        /// Recovery time constant of the health score, in seconds.
        /// The score drops instantly when the pre-decay score drops, but recovers
        /// toward 100 gradually: the remaining gap to 100 shrinks by a factor of
        /// exp(-DeltaT / tDecay) per frame. Zero or less disables decay.
        /// </param>
        /// <param name="hysteresisEntryValue">
        /// The values needed for a hysteresis based state machine:
        /// Health score needs to go below EntryValue to be considered alert worthy.
        /// </param>
        /// <param name="hysteresisExitValue">
        /// Health score needs to go back above ExitValue to stop constituting an alert.
        /// </param>
        public Fusion
        (
            double[] cusumLevels,
            double[] d2Levels,
            double d2Weight,
            double tDecay,
            double hysteresisEntryValue,
            double hysteresisExitValue
        )
        {
            _cusumLadder = new AnchorLadder(cusumLevels);
            _d2Ladder = new AnchorLadder(d2Levels);
            _d2Weight = d2Weight;
            _tDecay = tDecay;
            _hysteresisEntryValue = hysteresisEntryValue;
            _hysteresisExitValue = hysteresisExitValue;
        }

        public AlgorithmResult FusionDelegate(Tuple<MahalanobisResult, CusumResult> combinedResult)
        {
            _mahalanobisDistanceSquared = combinedResult.Item1.DistanceSquared;
            _deltaT = combinedResult.Item1.DeltaT;

            FuseCusums(combinedResult.Item2.AmountsAboveThresholds);

            Standardize();

            CalculateHealthScore();

            return BuildResult(combinedResult);
        }

        /// <summary>
        /// Calculates CusumRaw and saves it in the field.
        /// </summary>
        private void FuseCusums(List<double> amountsAboveThresholds)
        {
            double sum = 0;
            foreach (double excess in amountsAboveThresholds)
            {
                sum += Math.Max(0, excess);
            }

            _cusumRaw = sum;
        }

        /// <summary>
        /// Uses the fields for CusumRaw and squared Mahalanobis Distance, along with the levels of both,
        /// to calculate the standardize version of both.
        /// </summary>
        private void Standardize()
        {
            double d2Level = _d2Ladder.GetLevel(_mahalanobisDistanceSquared);
            double cusumLevel = _cusumLadder.GetLevel(_cusumRaw);

            _standardizedMahalanobis = LevelToScore(d2Level, _d2Ladder.LastIndex);
            _standardizedCusum = LevelToScore(cusumLevel, _cusumLadder.LastIndex);
        }

        /// <summary>
        /// Calculates the health score using weights and standardized values,
        /// then applies decay and recalculates InAlert using hysteresis.
        /// </summary>
        private void CalculateHealthScore()
        {
            double mahalanobisContribution = _d2Weight * _standardizedMahalanobis;
            double cusumContribution = (1 - _d2Weight) * _standardizedCusum;

            _preDecayHealthScore = mahalanobisContribution + cusumContribution;

            // Decay: drops are taken immediately, recovery is gradual (toward 100).
            double decayed = _preDecayHealthScore;
            if (_tDecay > 0)
            {
                double dt = Math.Max(0, _deltaT);
                double recovering = MaxScore - (MaxScore - _decayedHealthScore) * Math.Exp(-dt / _tDecay);
                decayed = Math.Min(_preDecayHealthScore, recovering);
            }

            _decayedHealthScore = decayed;
            _healthScore = (int)Math.Round(decayed);

            // Hysteresis on the decayed (unrounded) score.
            if (!_inAlert && decayed < _hysteresisEntryValue)
            {
                _inAlert = true;
            }
            else if (_inAlert && decayed > _hysteresisExitValue)
            {
                _inAlert = false;
            }
        }

        /// <summary>
        /// Level 0 (index 0) maps to 100, the top level maps to 1, linear in between.
        /// </summary>
        private static double LevelToScore(double level, int lastIndex)
        {
            double fraction = level / lastIndex;

            return MaxScore - (MaxScore - MinScore) * fraction;
        }

        private AlgorithmResult BuildResult(Tuple<MahalanobisResult, CusumResult> combinedResult)
        {
            MahalanobisResult m = combinedResult.Item1;
            CusumResult c = combinedResult.Item2;

            Dictionary<string, double> cusumAnomalies = [];
            string parameterName;
            double parameterValue;

            for (int i = 0; i < c.AnomalousParameterNames.Count; i++)
            {
                parameterName = c.AnomalousParameterNames[i];
                parameterValue = c.AnomalousParameterValues[i];
                cusumAnomalies.Add(parameterName, parameterValue);
            }

            return new AlgorithmResult()
            {
                HealthScore = _healthScore,
                MahalanobisAnomalies = combinedResult.Item1.OutlierParameters,
                CusumAnomalies = cusumAnomalies,
                InAlert = _inAlert
            };
        }    }
}