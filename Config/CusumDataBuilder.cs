using Microsoft.Extensions.Options;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;

namespace TelemetryHealthPredictor.Config
{
    public static class CusumDataBuilder
    {
        public static CusumData Build(IServiceProvider provider)
        {
            CusumSettings settings = provider.GetRequiredService<IOptions<CusumSettings>>().Value;

            double[] slacks = CSVReader.Load(nameof(settings.SlacksPath),
                settings.SlacksPath, CSVReader.ReadDoubles);

            double[] thresholds = CSVReader.Load(nameof(settings.ThresholdsPath),
                settings.ThresholdsPath, CSVReader.ReadDoubles);

            double[] cusumInitValues = CSVReader.Load(nameof(settings.CusumInitValuesPath),
                settings.CusumInitValuesPath, CSVReader.ReadDoubles);



            return new CusumData()
            {
                Slacks = slacks,
                Thresholds = thresholds,
                PositiveCusums = cusumInitValues,
                NegativeCusums = cusumInitValues
            };
        }
    }
}
