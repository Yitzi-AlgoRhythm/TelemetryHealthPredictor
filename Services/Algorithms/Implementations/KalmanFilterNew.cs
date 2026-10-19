using MathNet.Numerics.LinearAlgebra;
using TelemetryHealthPredictor.Model.AlgorithmDataObjects;
using TelemetryHealthPredictor.Model.AlgorithmResultObjects;
using TelemetryHealthPredictor.Services.Algorithms.Abstractions;
using TelemetryHealthPredictor.Services.Algorithms.Implementations.Extensions;

namespace TelemetryHealthPredictor.Services.Algorithms.Implementations
{
    public class KalmanFilterNew : IKalmanFilter
    {
        private const double MinVariance = 1e-9;

        // ---- Fixed configuration (set once in the constructor, never modified) ----
        private readonly Dictionary<string, int> _parameterIndices;
        private readonly Matrix<double> _vanLoanBase; // 2n x 2n matrix [[-A, G Qc G^T], [0, A^T]] (per second)
        private readonly double _b;             // Sage-Husa forgetting factor per second, in (0, 1)
        private readonly int _n;                // state dimension

        // ---- Filter state, carried across calls ----
        private Matrix<double> _Rk;             // full n x n measurement noise (adapted over time)
        private Matrix<double> _Pk;             // state covariance
        private Vector<double> _Xk;             // state estimate
        private ulong _lastTimestamp;           // filter clock (ms)
        private bool _hasTimestamp;

        // ---- Per-step working variables (overwritten every call) ----
        private Matrix<double> _Hk, _R_prev_k, _Fk, _Qk, _Sk, _P_prev_k, _Kk;
        private Vector<double> _Yk, _Zk, _X_prev_k, _Z_scores;

        /// <param name="parameterIndices">Maps each parameter name to its index in the state vector.</param>
        /// <param name="a">Continuous-time dynamics matrix A (n x n): rate of change of each variable per second.</param>
        /// <param name="g">Noise input matrix G (n x k): how each of the k noise sources enters the state.</param>
        /// <param name="qc">Noise intensities Qc (length k): variance per second of each independent noise source.</param>
        /// <param name="r0">Initial measurement noise covariance (n x n).</param>
        /// <param name="p0">Initial state covariance (n x n).</param>
        /// <param name="x0">Initial state estimate (length n).</param>
        /// <param name="forgettingFactor">
        /// Sage-Husa forgetting factor per second. Closer to 1 = longer memory.
        /// A value outside (0, 1) disables R adaptation.
        /// </param>
        public KalmanFilterNew
        (
            Dictionary<string, int> parameterIndices,
            Matrix<double> a,
            Matrix<double> g,
            Vector<double> qc,
            Matrix<double> r0,
            Matrix<double> p0,
            Vector<double> x0,
            double forgettingFactor
        )
        {
            ArgumentNullException.ThrowIfNull(parameterIndices);
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(g);
            ArgumentNullException.ThrowIfNull(qc);
            ArgumentNullException.ThrowIfNull(r0);
            ArgumentNullException.ThrowIfNull(p0);
            ArgumentNullException.ThrowIfNull(x0);

            _n = x0.Count;
            RequireSquare(a, _n, nameof(a));
            RequireSquare(r0, _n, nameof(r0));
            RequireSquare(p0, _n, nameof(p0));

            if (g.RowCount != _n || g.ColumnCount != qc.Count)
            {
                throw new ArgumentException(
                    $"Expected G to be {_n} x {qc.Count} (n x number of noise sources) but got {g.RowCount} x {g.ColumnCount}.",
                    nameof(g));
            }

            if (qc.Any(q => q < 0.0))
            {
                throw new ArgumentException("Noise intensities must be non-negative.", nameof(qc));
            }

            foreach (KeyValuePair<string, int> parameterIndex in parameterIndices)
            {
                if (parameterIndex.Value < 0 || parameterIndex.Value >= _n)
                {
                    throw new ArgumentException(
                        $"Index {parameterIndex.Value} for parameter '{parameterIndex.Key}' is outside the state dimension {_n}.",
                        nameof(parameterIndices));
                }
            }

            _parameterIndices = new Dictionary<string, int>(parameterIndices);
            _b = forgettingFactor;

            // Van Loan's matrix without the dt factor. Nothing in it depends on dt,
            // so it is built once here and only scaled by dt on every step.
            Matrix<double> noiseCovariance = g * Matrix<double>.Build.DiagonalOfDiagonalVector(qc) * g.Transpose();

            _vanLoanBase = Matrix<double>.Build.Dense(2 * _n, 2 * _n);
            _vanLoanBase.SetSubMatrix(0, 0, -a);
            _vanLoanBase.SetSubMatrix(0, _n, noiseCovariance);
            _vanLoanBase.SetSubMatrix(_n, _n, a.Transpose());

            _Rk = r0.Clone();
            _Pk = p0.Clone();
            _Xk = x0.Clone();

            // Working variables start as valid, empty/identity-sized values.
            // F and Q at dt = 0 are the identity and zero matrices.
            _Fk = Matrix<double>.Build.DenseIdentity(_n);
            _Qk = Matrix<double>.Build.Dense(_n, _n);
            _R_prev_k = _Rk.Clone();
            _P_prev_k = _Pk.Clone();
            _X_prev_k = _Xk.Clone();
            _Hk = Matrix<double>.Build.Dense(0, _n);
            _Sk = Matrix<double>.Build.Dense(0, 0);
            _Kk = Matrix<double>.Build.Dense(_n, 0);
            _Yk = Vector<double>.Build.Dense(0);
            _Zk = Vector<double>.Build.Dense(0);
            _Z_scores = Vector<double>.Build.Dense(0);
        }

        public KalmanFilterResult[] KalmanFilterDelegate(IncomingFlightParameter[] parameters)
        {
            int m = parameters.Length;
            int[] indices = new int[m];
            double[] values = new double[m];
            ulong newestTimestamp = 0;

            for (int i = 0; i < m; i++)
            {
                var p = parameters[i];
                indices[i] = _parameterIndices[p.Name];
                values[i] = p.Value;
                newestTimestamp = Math.Max(newestTimestamp, p.Timestamp);
            }

            double dt = ComputeDt(newestTimestamp);

            AdjustFAndQ(dt);
            PredictStep();
            BuildMeasurementModel(indices, values);
            ComputeInnovation();
            ComputeKalmanGainAndUpdate();
            ComputeZScores();
            UpdateRSageHusa(indices, dt);

            return new[] { new KalmanFilterResult(_Z_scores, indices) };
        }

        /// <summary>
        /// Seconds from the filter clock to the newest timestamp in this group.
        /// The first group initializes the clock (dt = 0). A group older than the
        /// filter clock is treated as dt = 0 and does not move the clock backwards.
        /// </summary>
        private double ComputeDt(ulong newestTimestamp)
        {
            if (!_hasTimestamp)
            {
                _lastTimestamp = newestTimestamp;
                _hasTimestamp = true;
                return 0.0;
            }

            if (newestTimestamp <= _lastTimestamp)
                return 0.0;

            double dt = (newestTimestamp - _lastTimestamp) / 1000.0;
            _lastTimestamp = newestTimestamp;
            return dt;
        }

        /// <summary>
        /// Rebuilds F(dt) and Q(dt) from the continuous-time model using Van Loan's method.
        /// With M = [[-A, G Qc G^T], [0, A^T]] * dt and E = exp(M) split into n x n blocks
        /// [[E11, E12], [0, E22]]:  F = E22^T  and  Q = F * E12.
        /// Always built from the fixed model, so nothing compounds across calls.
        /// At dt = 0 the result is exactly F = I and Q = 0, so the matrix exponential is skipped.
        /// </summary>
        private void AdjustFAndQ(double dt)
        {
            if (dt <= 0.0)
            {
                _Fk = Matrix<double>.Build.DenseIdentity(_n);
                _Qk = Matrix<double>.Build.Dense(_n, _n);
                return;
            }

            Matrix<double> exponential = (_vanLoanBase * dt).Exponential();

            _Fk = exponential.SubMatrix(_n, _n, _n, _n).Transpose();
            _Qk = Symmetrize(_Fk * exponential.SubMatrix(0, _n, _n, _n));
        }

        /// <summary>Predicts the full state and covariance forward over dt.</summary>
        private void PredictStep()
        {
            _X_prev_k = _Xk.Clone();
            _P_prev_k = _Pk.Clone();

            _Xk = _Fk * _Xk;
            _Pk = Symmetrize(_Fk * _Pk * _Fk.Transpose() + _Qk);
        }

        /// <summary>
        /// Builds the selection matrix H (m x n) and the measurement vector z (m),
        /// and snapshots R before this step's adaptation.
        /// </summary>
        private void BuildMeasurementModel(int[] indices, double[] values)
        {
            int m = indices.Length;

            _Hk = Matrix<double>.Build.Dense(m, _n);

            // H has a column per state variable and a row per measured parameter.
            // It maps for each measured parameter which state variable it represents.
            // It does that by having a 1 in each row (measured parameter)
            // only in the column of the state variable it represents.

            for (int row = 0; row < m; row++)
            {
                _Hk[row, indices[row]] = 1.0;
            }

            _Zk = Vector<double>.Build.DenseOfArray(values);
            _R_prev_k = _Rk.Clone();
        }

        /// <summary>Y = z - H x, S = H P H^T + R_measured.</summary>
        private void ComputeInnovation()
        {
            _Yk = _Zk - _Hk * _Xk;
            _Sk = Symmetrize(_Hk * _Pk * _Hk.Transpose() + ExtractMeasuredR(_Rk));
        }

        /// <summary>
        /// K = P H^T S^-1 (computed via a linear solve), then X and P are updated.
        /// Covariance uses the Joseph form so it stays symmetric positive semi-definite.
        /// </summary>
        private void ComputeKalmanGainAndUpdate()
        {
            // S is symmetric, so K^T = S^-1 (H P)  =>  K = (S.Solve(H P))^T
            _Kk = _Sk.Solve(_Hk * _Pk).Transpose();

            _Xk = _Xk + _Kk * _Yk;

            Matrix<double> identity = Matrix<double>.Build.DenseIdentity(_n);
            Matrix<double> iMinusKH = identity - _Kk * _Hk;
            Matrix<double> rMeasured = ExtractMeasuredR(_Rk);

            _Pk = Symmetrize(iMinusKH * _Pk * iMinusKH.Transpose() + _Kk * rMeasured * _Kk.Transpose());
        }

        /// <summary>Z-score per measured variable: Y_i / sqrt(S_ii). Aligned with IndicesMeasured.</summary>
        private void ComputeZScores()
        {
            _Z_scores = Vector<double>.Build.Dense(_Yk.Count, i =>
            {
                double s = _Sk[i, i];
                return s > 0 ? _Yk[i] / Math.Sqrt(s) : 0.0;
            });
        }

        /// <summary>
        /// Sage-Husa adaptive update of the measured diagonal entries of R:
        ///   r_ii = (1 - d) * r_ii + d * (Y_i^2 - (H P- H^T)_ii),  d = 1 - b^dt
        /// The forgetting factor b is per second, so it is adjusted by dt. R is floored to stay positive.
        /// </summary>
        private void UpdateRSageHusa(int[] indices, double dt)
        {
            if (_b <= 0.0 || _b >= 1.0 || dt <= 0.0)
                return;

            double d = 1.0 - Math.Pow(_b, dt);

            // H P- H^T = S - R (both taken from before this update)
            Matrix<double> hphT = _Sk - ExtractMeasuredR(_R_prev_k);

            for (int i = 0; i < indices.Length; i++)
            {
                int idx = indices[i];
                double estimate = _Yk[i] * _Yk[i] - hphT[i, i];
                double updated = (1.0 - d) * _R_prev_k[idx, idx] + d * estimate;
                _Rk[idx, idx] = Math.Max(updated, MinVariance);
            }
        }

        /// <summary>Extracts the m x m block of a full n x n R for the measured indices (via H).</summary>
        private Matrix<double> ExtractMeasuredR(Matrix<double> fullR)
        {
            return _Hk * fullR * _Hk.Transpose();
        }

        private static Matrix<double> Symmetrize(Matrix<double> a)
        {
            return (a + a.Transpose()) * 0.5;
        }

        private static void RequireSquare(Matrix<double> matrix, int n, string paramName)
        {
            if (matrix.RowCount != n || matrix.ColumnCount != n)
                throw new ArgumentException(
                    $"Expected a {n} x {n} matrix but got {matrix.RowCount} x {matrix.ColumnCount}.",
                    paramName);
        }
    }
}
