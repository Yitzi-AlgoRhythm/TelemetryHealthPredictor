using System.Text.Json;
using ParameterDataLib;

namespace TelemetryHealthPredictor.Services.Kafka
{
    public class Decoder
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
