using TelemetryHealthPredictor.Algorithm;
using TelemetryHealthPredictor.Algorithm.Delegates.Abstractions;
using TelemetryHealthPredictor.Algorithm.Delegates.Implementations;
using TelemetryHealthPredictor.Algorithm.Delegates.Mockups;
using TelemetryHealthPredictor.Config;
using TelemetryHealthPredictor.Config.Builders;
using TelemetryHealthPredictor.Config.Settings;
using TelemetryHealthPredictor.Kafka;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;
using TelemetryHealthPredictor.Services.Algorithms.Mockups;

namespace TelemetryHealthPredictor.Setup.BuilderExtensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddSignalR();

            builder.Services.Configure<KafkaConsumerConfig>
                (builder.Configuration.GetSection(nameof(KafkaConsumerConfig)));

            builder.Services.Configure<NamingSettings>
                (builder.Configuration.GetSection(nameof(NamingSettings)));
            builder.Services.AddSingleton(NamingDataBuilder.Build);

            builder.Services.Configure<KalmanSettings>
                (builder.Configuration.GetSection(nameof(KalmanSettings)));
            builder.Services.AddSingleton(KalmanBuilder.Build);

            builder.Services.Configure<MahalanobisSettings>
                (builder.Configuration.GetSection(nameof(MahalanobisSettings)));
            builder.Services.AddSingleton(MahalanobisBuilder.Build);

            builder.Services.Configure<CusumSettings>
                (builder.Configuration.GetSection(nameof(CusumSettings)));
            builder.Services.AddSingleton(CusumBuilder.Build);

            builder.Services.AddSingleton<IKalmanFilter, KalmanFilter>();
            builder.Services.AddSingleton<IMahalanobis, Mahalanobis>();
            builder.Services.AddSingleton<ICusum, Cusum>();
            builder.Services.AddSingleton<IFusion, FusionMockup>();
            builder.Services.AddSingleton<IResultSender, ResultSender>();

            builder.Services.AddSingleton<IPipeline, Pipeline>();

            builder.Services.AddHostedService<KafkaConsumerService>();

            return builder;
        }
    }
}
