using System.Text.Json;
using ParameterDataLib;
using TelemetryHealthPredictor.Services.Algorithms.Abstractions;

namespace TelemetryHealthPredictor.Services.Algorithms.Implementations
{
    public class Decoder : IDecoder
    {
        public KeyValuePair<string, double> DecoderDelegate(string json)
        {
            ParameterData parameterData = JsonSerializer.Deserialize<ParameterData>(json)!;

            return new KeyValuePair<string, double>
            (
                parameterData.Name,
                parameterData.Value
            );
        }
    }
}
