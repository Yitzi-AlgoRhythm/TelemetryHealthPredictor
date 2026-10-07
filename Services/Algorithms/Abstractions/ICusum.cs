using MathNet.Numerics.LinearAlgebra;

namespace TelemetryHealthPredictor.Services.Algorithms.Abstractions
{
    public interface ICusum
    {
        public Dictionary<string, double> CusumDelegate(Vector<double> filteredResult);
    }
}
