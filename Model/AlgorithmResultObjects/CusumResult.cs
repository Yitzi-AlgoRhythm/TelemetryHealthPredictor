namespace TelemetryHealthPredictor.Model.AlgorithmResultObjects
{
    public class CusumResult
    {
        public Dictionary<string, double> AnomalousParameterCusums { get; init; }

        public CusumResult(Dictionary<string, double> cusums)
        {
            AnomalousParameterCusums = cusums;
        }
    }
}
