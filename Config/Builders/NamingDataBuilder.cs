using Microsoft.Extensions.Options;
using TelemetryHealthPredictor.Config.Settings;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;

namespace TelemetryHealthPredictor.Config.Builders
{
    public class NamingDataBuilder
    {
        public static NamingData Build(IServiceProvider provider)
        {
            NamingSettings settings = provider.GetRequiredService<IOptions<NamingSettings>>().Value;

            string[] variableNames = CSVReader.Load(nameof(settings.VariablesPath),
                settings.VariablesPath, p => CSVReader.ReadStrings(p));

            return new NamingData()
            {
                ParameterNames = variableNames 
            };
        }
    }
}
