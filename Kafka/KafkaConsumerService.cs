using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using ParameterDataLib;
using TelemetryHealthPredictor.Config;
using TelemetryHealthPredictor.Algorithm;
using TelemetryHealthPredictor.Data;

namespace TelemetryHealthPredictor.Kafka
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
                            string json = result.Message.Value;

                            ParameterData[] incomingParameterData = JsonSerializer.Deserialize<ParameterData[]>(json)!;

                            IncomingFlightParameter[] incomingParameters = Array.ConvertAll(incomingParameterData, data =>
                            {
                                return new IncomingFlightParameter(data);
                            });

                            _pipeline.BeginAlgorithm(incomingParameters);
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
