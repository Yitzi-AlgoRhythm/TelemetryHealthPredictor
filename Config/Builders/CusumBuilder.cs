using Microsoft.Extensions.Options;
using TelemetryHealthPredictor.Algorithm.Delegates.Implementations;
using TelemetryHealthPredictor.Config.Settings;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;

namespace TelemetryHealthPredictor.Config.Builders
{
    public static class CusumBuilder
    {
        public static Cusum Build(IServiceProvider provider)
        {
            CusumSettings settings = provider.GetRequiredService<IOptions<CusumSettings>>().Value;

            NamingData namingData = provider.GetRequiredService<NamingData>();

            double[] slacks = CSVReader.Load(nameof(settings.SlacksPath),
                settings.SlacksPath, CSVReader.ReadDoubles);

            double[] thresholds = CSVReader.Load(nameof(settings.ThresholdsPath),
                settings.ThresholdsPath, CSVReader.ReadDoubles);



            return new Cusum
            (
                namingData,
                slacks,
                thresholds
            );
        }
    }
}
