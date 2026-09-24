namespace TelemetryHealthPredictor.Config
{
    public class KafkaConsumerConfig
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public string GroupID {  get; set; } = string.Empty;
    }
}
