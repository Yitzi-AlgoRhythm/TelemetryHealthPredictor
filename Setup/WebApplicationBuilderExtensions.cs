using TelemetryHealthPredictor.Config;
using TelemetryHealthPredictor.Services.Connection;

namespace TelemetryHealthPredictor.Setup
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();

            builder.Services.Configure<KafkaConsumerConfig>
                (builder.Configuration.GetSection(nameof(KafkaConsumerConfig)));

            builder.Services.AddSingleton<WebsocketService>();

            builder.Services.Configure<KalmanSettings>
                (builder.Configuration.GetSection(nameof(KalmanSettings)));
            builder.Services.AddSingleton(KalmanSetupDataBuilder.Build);

            //builder.Services.AddSingleton<ILoggerTD, ConsoleLogger>();

            return builder;
        }
    }
}
