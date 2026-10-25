using TelemetryHealthPredictor.Data;

namespace TelemetryHealthPredictor.Algorithm
{
    public interface IPipeline
    {
        public Task Completion();

        public void BeginAlgorithm(IncomingFlightParameter[] parameter);

        public void Complete();

        public void Fault(Exception ex);
    }
}
