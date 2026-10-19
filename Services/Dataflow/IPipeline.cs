using TelemetryHealthPredictor.Model.AlgorithmDataObjects;

namespace TelemetryHealthPredictor.Services.Dataflow
{
    public interface IPipeline
    {
        public Task Completion();

        public void BeginAlgorithm(IncomingFlightParameter[] parameter);

        public void Complete();

        public void Fault(Exception ex);
    }
}
