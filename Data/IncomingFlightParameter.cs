using ParameterDataLib;

namespace TelemetryHealthPredictor.Data
{
    public class IncomingFlightParameter
    {
        public string Name { get; init; }
        public double Value { get; init; }
        public ulong Timestamp { get; init; }

        public IncomingFlightParameter(ParameterData parameterData)
        {
            Name = parameterData.Name;
            Value = parameterData.Value;
            Timestamp = parameterData.TimestampMS;
        }
    }
}
