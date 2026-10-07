using Microsoft.Extensions.Options;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;

namespace TelemetryHealthPredictor.Config
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
                VariableNames = variableNames 
            };
        }
    }
}
