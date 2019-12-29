using System.Threading;
using System.Threading.Tasks;
using MathCore.Annotations;
using MathCore.Values;
using MathCore.ViewModels;

namespace System.Collections.Generic
{
    public class FunctionManager<T> : ViewModel
    {
        private readonly double _X1;
        private readonly double _X2;

        private static double GetEps(double x1, double x2, double y1, double y2)
        {
            x2 -= x1;
            y2 -= y1;
            return x2 * x2 + y2 * y2;
        }

        [NotNull] private readonly Func<T, double> _DoubleConverter;
        [NotNull] private readonly Func<double, T> _Function;
        private readonly ObservableLinkedList<KeyValuePair<double, T>> _Values = new ObservableLinkedList<KeyValuePair<double, T>>();
        private double _Eps0;
        private double _Min = double.NaN;
        private double _Max = double.NaN;
        private double _Integral = double.NaN;

        public double Eps0 => _Eps0;
        public double Min => _Min;
        public double Max => _Max;
        public double Integral => _Integral;

        public ObservableLinkedList<KeyValuePair<double, T>> Values => _Values;

        public FunctionManager([NotNull] Func<double, T> function, double x1, double x2, [NotNull] Func<T, double> double_converter)
        {
            _Function = function ?? throw new ArgumentNullException(nameof(function));
            _DoubleConverter = double_converter ?? throw new ArgumentNullException(nameof(double_converter));
            _X1 = x1;
            _X2 = x2;
        }

        public void Reset()
        {
            Initialize();
            OnPropertyChanged(nameof(Min));
            OnPropertyChanged(nameof(Max));
            OnPropertyChanged(nameof(Integral));
            OnPropertyChanged(nameof(Eps0));
        }

        private void Initialize()
        {
            if (_Values.Count > 0) _Values.Clear();
            var x1 = _X1;
            var v1 = _Function(x1);
            var x2 = _X2;
            var v2 = _Function(x2);
            _Values.Add(new KeyValuePair<double, T>(x1, v1));
            _Values.Add(new KeyValuePair<double, T>(x2, v2));
            var y1 = _DoubleConverter(v1);
            var y2 = _DoubleConverter(v2);
            _Eps0 = GetEps(x1, x2, y1, y2);
            _Integral = (y1 + y2) / 2 * (x2 - x1);
            _Min = Math.Min(y1, y2);
            _Max = Math.Max(y1, y2);
        }

        public double SetEps(double eps0)
        {
            if (_Values.Count < 2) Initialize();
            var n1 = _Values.First;
            var n2 = n1.Next;

            var I = 0d;
            var v1 = n1.Value;
            var x1 = v1.Key;
            var y1 = _DoubleConverter(v1.Value);
            var eps1 = 0d;
            var min_max = new MinMaxValue();
            var dI = y1 / 2;
            min_max.AddValue(y1);
            do
            {
                var v2 = n2.Value;
                var x2 = v2.Key;
                var y2 = _DoubleConverter(v2.Value);
                min_max.AddValue(y2);

                var eps = GetEps(x1, x2, y1, y2);

                if (eps < eps0)
                {
                    if (eps > eps1) eps1 = eps;
                    n1 = n2;
                    dI += y2 / 2;
                    I += dI * (x2 - x1);
                    dI = y2 / 2;
                    y1 = y2;
                    x1 = x2;
                    n2 = n1.Next;
                    continue;
                }

                x2 = (x1 + x2) / 2;
                v2 = new KeyValuePair<double, T>(x2, _Function(x2));
                n2 = _Values.AddAfter(n1, v2);
            } while (n2 != null);

            if (eps1.Equals(0d)) eps1 = eps0;
            Set(ref _Eps0, eps1, nameof(Eps0));
            Set(ref _Min, min_max.Min, nameof(Min));
            Set(ref _Max, min_max.Max, nameof(Max));
            Set(ref _Integral, I, nameof(Integral));
            return eps1;
        }

        public async Task<double> SetEpsAsync(double eps0, IProgress<double> progress = null, CancellationToken cancel = default)
        {
            await Task.Yield();
            if (_Values.Count < 2) await Task.Factory.StartNew(Initialize, cancel).ConfigureAwait(true);

            var n1 = _Values.First;
            var n2 = n1.Next;

            var I = 0d;
            var v1 = n1.Value;
            var x1 = v1.Key;
            var y1 = _DoubleConverter(v1.Value);
            var eps1 = 0d;
            var min_max = new MinMaxValue();
            var dI = y1 / 2;
            min_max.AddValue(y1);
            var Dx = _X2 - _X1;
            progress?.Report(0d);
            do
            {
                var v2 = n2.Value;
                var x2 = v2.Key;
                var y2 = _DoubleConverter(v2.Value);
                min_max.AddValue(y2);

                var eps = GetEps(x1, x2, y1, y2);

                if (eps < eps0)
                {
                    if (eps > eps1) eps1 = eps;
                    n1 = n2;
                    dI += y2 / 2;
                    I += dI * (x2 - x1);
                    dI = y2 / 2;
                    y1 = y2;
                    x1 = x2;
                    n2 = n1.Next;
                    progress?.Report((x1 - _X1) / Dx);
                    continue;
                }

                x2 = (x1 + x2) / 2;
                v2 = new KeyValuePair<double, T>(x2, _Function(x2));
                n2 = _Values.AddAfter(n1, v2);
            } while (n2 != null);
            progress?.Report(1d);
            if (eps1.Equals(0d)) eps1 = eps0;
            Set(ref _Eps0, eps1, nameof(Eps0));
            Set(ref _Min, min_max.Min, nameof(Min));
            Set(ref _Max, min_max.Max, nameof(Max));
            Set(ref _Integral, I, nameof(Integral));
            return eps1;
        }
    }
}