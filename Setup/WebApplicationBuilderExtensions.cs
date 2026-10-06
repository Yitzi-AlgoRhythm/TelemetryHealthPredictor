using TelemetryHealthPredictor.Config;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;
using TelemetryHealthPredictor.Services.Connection;

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

            builder.Services.AddSingleton<BroadcastService>();

            return builder;
        }
    }
}
