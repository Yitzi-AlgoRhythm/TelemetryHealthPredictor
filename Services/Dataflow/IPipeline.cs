namespace TelemetryHealthPredictor.Services.Dataflow
{
    public interface IPipeline
    {
        public Task Completion();

        public void BeginAlgorithm(string json);

        public void Complete();

        public void Fault(Exception ex);
    }
}
