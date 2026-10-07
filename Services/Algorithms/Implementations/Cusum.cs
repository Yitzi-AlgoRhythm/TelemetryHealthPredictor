using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;
using TelemetryHealthPredictor.Services.Algorithms.Abstractions;

namespace TelemetryHealthPredictor.Services.Algorithms.Implementations
{
    public class Cusum : ICusum
    {
        private readonly CusumData _cusumData;

        public Cusum(CusumData cusumData)
        {
            _cusumData = cusumData;
        }

        public Dictionary<string, double> CusumDelegate(Vector<double> filteredResult)
        {
            return _cusumData.GetAnomalousCusums(filteredResult);
        }
    }
}
