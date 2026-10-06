namespace TelemetryHealthPredictor.Services.Algorithms.Abstractions
{
    public interface IDecoder
    {
        public KeyValuePair<string, double> DecoderDelegate(string json);
    }
}
