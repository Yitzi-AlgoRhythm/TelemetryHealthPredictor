using Confluent.Kafka;
using Microsoft.Extensions.Options;
using TelemetryHealthPredictor.Config;
using TelemetryHealthPredictor.Services.Dataflow;

namespace TelemetryHealthPredictor.Services.Kafka
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IPipeline _pipeline;

        private readonly KafkaConsumerConfig _configDetails;

        public KafkaConsumerService(IPipeline pipeline, IOptions<KafkaConsumerConfig> options)
        {
            _pipeline = pipeline;
            _configDetails = options.Value;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => Begin(stoppingToken), stoppingToken);
        }

        private async Task Begin(CancellationToken token)
        {
            ConsumerConfig consumerConfig = new()
            {
                BootstrapServers = _configDetails.BootstrapServers,
                GroupId = _configDetails.GroupID,
                AutoOffsetReset = AutoOffsetReset.Latest
            };

            using IConsumer<string, string> consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();

            consumer.Subscribe(_configDetails.TopicName);

            try
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        ConsumeResult<string, string> result = consumer.Consume(token);

                        if (result.Message.Value is not null)
                        {
                            _pipeline.BeginAlgorithm(result.Message.Value);
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine();
                        Console.WriteLine(ex.StackTrace);

                        await Task.Delay(2000);
                    }
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                consumer.Close();
                _pipeline.Complete();
                await _pipeline.Completion();
            }
        }
    }
}
