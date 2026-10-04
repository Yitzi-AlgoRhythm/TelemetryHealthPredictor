namespace TelemetryHealthPredictor.Model.AlgorithmDataObjects
{
    public class CusumData
    {
        public required double[] Slacks { get; init; }
        public required double[] Thresholds { get; init; }

        public required double[] PositiveCusums { get; set; }
        public required double[] NegativeCusums { get; set; }
    }
}
