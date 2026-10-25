using MathNet.Numerics.LinearAlgebra;

namespace TelemetryHealthPredictor.Algorithm.Delegates.Helpers
{
    public static class MatrixExtensions
    {
        // Order of the diagonal Padé approximant (degree of numerator and denominator).
        private const int PadeOrder = 6;

        /// <summary>
        /// Computes the matrix exponential exp(A) using scaling and squaring
        /// with a diagonal Padé approximant.
        /// </summary>
        public static Matrix<double> Exponential(this Matrix<double> matrix)
        {
            int dimension = matrix.ValidateSquare();
            int squarings = matrix.GetRequiredSquarings();

            // Shrink the matrix so the Padé approximation is accurate: x = A / 2^s.
            Matrix<double> scaled = matrix / Math.Pow(2.0, squarings);

            Matrix<double> scaledExponential = scaled.PadeExponential();

            return scaledExponential.RepeatedlySquare(squarings);
        }

        /// <summary>
        /// Throws if the matrix is not square; otherwise returns its dimension.
        /// </summary>
        private static int ValidateSquare(this Matrix<double> matrix)
        {
            int dimension = matrix.RowCount;
            if (dimension != matrix.ColumnCount)
                throw new ArgumentException("Matrix must be square.", nameof(matrix));

            return dimension;
        }

        /// <summary>
        /// Returns the number of halvings needed so the scaled matrix has
        /// infinity norm at most 1/2.
        /// </summary>
        private static int GetRequiredSquarings(this Matrix<double> matrix)
        {
            double norm = matrix.InfinityNorm();

            if (!double.IsFinite(norm))
                throw new ArgumentException("Matrix must contain only finite values.", nameof(matrix));

            return norm > 0.5 ? (int)Math.Ceiling(Math.Log2(norm)) + 1 : 0;
        }

        /// <summary>
        /// Approximates exp(x) for a small matrix x as D(x)^-1 * N(x), where
        ///   N(x) = sum c_k x^k,  D(x) = sum (-1)^k c_k x^k,
        ///   c_0 = 1,  c_k = c_(k-1) * (q - k + 1) / (k * (2q - k + 1)).
        /// </summary>
        private static Matrix<double> PadeExponential(this Matrix<double> matrix)
        {
            (Matrix<double> numerator, Matrix<double> denominator) = matrix.BuildPadePolynomials();

            // D^-1 * N, computed with a linear solve rather than an explicit inverse.
            return denominator.Solve(numerator);
        }

        /// <summary>
        /// Builds the numerator N(x) and denominator D(x) polynomials of the Padé approximant.
        /// Both share the same coefficients; D flips the sign of the odd powers.
        /// </summary>
        private static (Matrix<double> Numerator, Matrix<double> Denominator) BuildPadePolynomials(this Matrix<double> x)
        {
            int dimension = x.ValidateSquare();

            Matrix<double> identity = Matrix<double>.Build.DenseIdentity(dimension);

            double coefficient = 0.5; // c_1
            Matrix<double> numerator = identity + coefficient * x;
            Matrix<double> denominator = identity - coefficient * x;
            Matrix<double> power = x; // holds x^k

            for (int k = 2; k <= PadeOrder; k++)
            {
                coefficient *= GetNextCoefficientRatio(k);
                power = x * power;
                Matrix<double> term = coefficient * power;

                numerator += term;

                if (k % 2 == 0) denominator += term;
                else denominator -= term;
            }

            return (numerator, denominator);
        }

        /// <summary>
        /// Ratio c_k / c_(k-1) for the Padé coefficients: (q - k + 1) / (k * (2q - k + 1)).
        /// </summary>
        private static double GetNextCoefficientRatio(int k)
        {
            return (double)(PadeOrder - k + 1) / (k * (2 * PadeOrder - k + 1));
        }

        /// <summary>
        /// Undoes the scaling: exp(A) = exp(A / 2^s)^(2^s), by squaring s times.
        /// </summary>
        private static Matrix<double> RepeatedlySquare(this Matrix<double> matrix, int times)
        {
            Matrix<double> result = matrix;

            for (int i = 0; i < times; i++)
                result = result * result;

            return result;
        }
    }
}