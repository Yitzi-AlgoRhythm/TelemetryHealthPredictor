using TelemetryHealthPredictor.Config;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;
using TelemetryHealthPredictor.Services.Algorithms.Abstractions;
using TelemetryHealthPredictor.Services.Algorithms.Implementations;
using TelemetryHealthPredictor.Services.Algorithms.Mockups;
using TelemetryHealthPredictor.Services.Dataflow;
using TelemetryHealthPredictor.Services.Kafka;

namespace TelemetryHealthPredictor.Setup
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddSignalR();

            builder.Services.Configure<KafkaConsumerConfig>
                (builder.Configuration.GetSection(nameof(KafkaConsumerConfig)));

            builder.Services.Configure<KalmanSettings>
                (builder.Configuration.GetSection(nameof(KalmanSettings)));
            builder.Services.AddSingleton(KalmanDataBuilder.Build);

            builder.Services.Configure<CusumSettings>
                (builder.Configuration.GetSection(nameof(CusumSettings)));
            builder.Services.AddSingleton(CusumDataBuilder.Build);

            builder.Services.AddSingleton<MahalanobisData>();

            builder.Services.AddSingleton<IDecoder, Decoder>();
            builder.Services.AddSingleton<IKalmanFilter, KalmanFilterMockup>();
            builder.Services.AddSingleton<IMahalanobis, MahalanobisMockup>();
            builder.Services.AddSingleton<ICusum, CusumMockup>();
            builder.Services.AddSingleton<IFusion, FusionMockup>();
            builder.Services.AddSingleton<IResultSender, BroadcastService>();

            builder.Services.AddSingleton<IPipeline, Pipeline>();

            builder.Services.AddHostedService<KafkaConsumerService>();

            return builder;
        }
    }
}
