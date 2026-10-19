using MathNet.Numerics.LinearAlgebra;

namespace TelemetryHealthPredictor.Services.Algorithms.Implementations.Extensions
{
    public static class MatrixExtensions
    {
        public static Matrix<double> Exponential(this Matrix<double> a)
        {
            int n = a.RowCount;
            if (n != a.ColumnCount)
                throw new ArgumentException("Matrix must be square.", nameof(a));

            // Scale so that the infinity norm is at most 1/2.
            double norm = a.InfinityNorm();
            int squarings = norm > 0.0
                ? Math.Max(0, 1 + (int)Math.Floor(Math.Log2(norm)))
                : 0;
            Matrix<double> x = a / Math.Pow(2.0, squarings);

            // Diagonal Padé approximant of order q: exp(x) ≈ D(x)^-1 N(x), where
            //   N(x) = sum c_k x^k,  D(x) = sum (-1)^k c_k x^k,
            //   c_0 = 1,  c_k = c_(k-1) * (q - k + 1) / (k * (2q - k + 1)).
            const int q = 6;
            Matrix<double> identity = Matrix<double>.Build.DenseIdentity(n);
            double c = 0.5;                                   // c_1
            Matrix<double> numerator = identity + c * x;
            Matrix<double> denominator = identity - c * x;
            Matrix<double> xPower = x;

            for (int k = 2; k <= q; k++)
            {
                c *= (double)(q - k + 1) / (k * (2 * q - k + 1));
                xPower = x * xPower;
                Matrix<double> term = c * xPower;
                numerator += term;
                if (k % 2 == 0) denominator += term;
                else denominator -= term;
            }

            // exp(x) ≈ D^-1 N, computed with a linear solve (not an explicit inverse).
            Matrix<double> result = denominator.Solve(numerator);

            // Undo the scaling: exp(a) = exp(a / 2^s)^(2^s).
            for (int i = 0; i < squarings; i++)
                result = result * result;

            return result;
        }
    }
}
