namespace TelemetryHealthPredictor.Algorithm.Delegates.Helpers
{
    /// <summary>
    /// Maps a raw value to a continuous level (the index scale of the anchor list).
    /// Ties in the anchors (e.g. CUSUM being exactly 0 in many frames) are collapsed:
    /// exactly at the tied value the level is the lowest tied index,
    /// anything above it jumps to the highest tied index.
    /// </summary>
    /// <summary>
    /// Maps a raw value to a continuous level (the index scale of the anchor list).
    /// Ties in the anchors (e.g. CUSUM being exactly 0 in many frames) are collapsed:
    /// exactly at the tied value the level is the lowest tied index,
    /// anything above it jumps to the highest tied index.
    /// </summary>
    public class AnchorLadder
    {
        private readonly double[] _distinctAnchorValues;          // distinct anchor values, increasing
        private readonly double[] _lowestIndexPerAnchorValue;     // lowest index sharing that value
        private readonly double[] _highestIndexPerAnchorValue;    // highest index sharing that value

        public int LastIndex { get; }

        public AnchorLadder(double[] anchorValues)
        {
            LastIndex = anchorValues.Length - 1;

            List<double> distinctValues = [];
            List<double> lowestIndices = [];
            List<double> highestIndices = [];

            for (int anchorIndex = 0; anchorIndex < anchorValues.Length; anchorIndex++)
            {
                if (distinctValues.Count > 0 && anchorValues[anchorIndex] == distinctValues[^1])
                {
                    highestIndices[^1] = anchorIndex;
                }
                else
                {
                    distinctValues.Add(anchorValues[anchorIndex]);
                    lowestIndices.Add(anchorIndex);
                    highestIndices.Add(anchorIndex);
                }
            }

            _distinctAnchorValues = distinctValues.ToArray();
            _lowestIndexPerAnchorValue = lowestIndices.ToArray();
            _highestIndexPerAnchorValue = highestIndices.ToArray();
        }

        public double GetLevel(double rawValue)
        {
            int lastDistinctIndex = _distinctAnchorValues.Length - 1;

            // at or below the first anchor: level 0
            if (rawValue <= _distinctAnchorValues[0])
            {
                return _lowestIndexPerAnchorValue[0];
            }

            // at or above the top anchor: capped
            if (rawValue >= _distinctAnchorValues[lastDistinctIndex])
            {
                return _highestIndexPerAnchorValue[lastDistinctIndex];
            }

            // Find segment segmentIndex with _distinctAnchorValues[segmentIndex] <= rawValue < _distinctAnchorValues[segmentIndex + 1].
            int segmentIndex = 0;
            while (_distinctAnchorValues[segmentIndex + 1] <= rawValue)
            {
                segmentIndex++;
            }

            if (rawValue == _distinctAnchorValues[segmentIndex])
            {
                return _lowestIndexPerAnchorValue[segmentIndex];
            }

            double fractionAlongSegment = (rawValue - _distinctAnchorValues[segmentIndex])
                                        / (_distinctAnchorValues[segmentIndex + 1] - _distinctAnchorValues[segmentIndex]);

            // Takes the level found for the value and linearly adds how far along it is towards the next level.
            // Multiplied by the lowest index of the next level minus the highest index of this level, to account for equal level values.
            return _highestIndexPerAnchorValue[segmentIndex]
                 + fractionAlongSegment * (_lowestIndexPerAnchorValue[segmentIndex + 1] - _highestIndexPerAnchorValue[segmentIndex]);
        }
    }
}
