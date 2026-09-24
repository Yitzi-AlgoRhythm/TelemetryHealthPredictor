using TelemetryHealthPredictor.Config;

namespace TelemetryHealthPredictor.Setup
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();

            builder.Services.Configure<KafkaConsumerConfig>
                (builder.Configuration.GetSection(nameof(KafkaConsumerConfig)));

            //builder.Services.AddSingleton<ILoggerTD, ConsoleLogger>();

            //builder.Services.AddSingleton<IcdDeserializer>();

            return builder;
        }
    }
}
