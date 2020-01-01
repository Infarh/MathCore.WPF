using System;
using MathCore.Annotations;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable VirtualMemberNeverOverridden.Global

namespace MathCore.WPF.Converters
{
    /// <summary>Простой математический конвертер для бинарных операций с константой (либо с параметром)</summary>
    public abstract class SimpleDoubleValueConverter : DoubleValueConverter
    {
        /// <summary>Метод преобразования значения</summary>
        /// <param name="value">Преобразуемое значение</param>
        /// <param name="parameter">Параметр преобразования</param>
        /// <returns>Преобразованное значение</returns>
        protected delegate double Conversion(double value, double parameter);

        /// <summary>Метод прямого преобразования</summary>
        private readonly Conversion _To;

        /// <summary>Метод обратного преобразования</summary>
        private readonly Conversion _From;

        /// <summary>Параметр преобразования</summary>
        public double Parameter { get; set; }

        protected SimpleDoubleValueConverter(double Parameter, Conversion? to = null, Conversion? from = null)
            : this(to, from) => this.Parameter = Parameter;

        protected SimpleDoubleValueConverter([CanBeNull] Conversion? to = null, [CanBeNull] Conversion? from = null)
        {
            _To = to ?? To;
            _From = from ?? From;
        }

        /// <summary>Прямое преобразование значения</summary>
        /// <param name="v">Преобразуемое значение</param>
        /// <param name="p">Возможный параметр преобразования. В случае отсутствия берётся значение параметра объекта</param>
        /// <returns>Преобразованное значение</returns>
        protected virtual double To(double v, double p) => throw new NotImplementedException("Прямое преобразование не реализовано");

        /// <summary>Обратное преобразование значения</summary>
        /// <param name="v">Преобразованное значение</param>
        /// <param name="p">Возможный параметр преобразования. В случае отсутствия берётся значение параметра объекта</param>
        /// <returns>Исходное значение</returns>
        protected virtual double From(double v, double p) => throw new NotSupportedException("Обратное преобразование не поддерживается");

        /// <inheritdoc />
        protected override double Convert(double v, double? p = null) => _To(v, p ?? Parameter);

        /// <inheritdoc />
        protected override double ConvertBack(double v, double? p = null) => _From(v, p ?? Parameter);
    }
}