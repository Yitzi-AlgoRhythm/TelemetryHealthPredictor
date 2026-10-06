using System.Globalization;
using MathNet.Numerics.Data.Text;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.VisualBasic.FileIO;

namespace TelemetryHealthPredictor.Config
{
    public static class CSVReader
    {
        public static Matrix<double> ReadMatrix(string csvPath)
        {
            return DelimitedReader.Read<double>(
                csvPath,
                delimiter: ",",
                hasHeaders: false,
                formatProvider: CultureInfo.InvariantCulture);
        }

        public static Vector<double> ReadVector(string csvPath)
        {
            return Vector<double>.Build.DenseOfEnumerable(ReadMatrix(csvPath).Enumerate());
        }

        public static double[] ReadDoubles(string csvPath)
        {
            return ReadMatrix(csvPath).Enumerate().ToArray();
        }

        public static string[] ReadStrings(string csvPath)
        {
            using TextFieldParser parser = new TextFieldParser(csvPath)
            {
                Delimiters = [","]
            };

            List<string> values = [];

            while (!parser.EndOfData)
            {
                values.AddRange(parser.ReadFields()!);
            }

            return values.ToArray();
        }

        public static T Load<T>(string settingName, string path, Func<string, T> reader)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"{settingName}: file not found at '{path}'.", path);

            return reader(path);
        }
    }
}
