using System;
using MathCore.Annotations;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable VirtualMemberNeverOverridden.Global

namespace MathCore.WPF.Converters
{
    /// <summary>������� �������������� ��������� ��� �������� �������� � ���������� (���� � ����������)</summary>
    public abstract class SimpleDoubleValueConverter : DoubleValueConverter
    {
        /// <summary>����� �������������� ��������</summary>
        /// <param name="value">������������� ��������</param>
        /// <param name="parameter">�������� ��������������</param>
        /// <returns>��������������� ��������</returns>
        protected delegate double Conversion(double value, double parameter);

        /// <summary>����� ������� ��������������</summary>
        private readonly Conversion _To;

        /// <summary>����� ��������� ��������������</summary>
        private readonly Conversion _From;

        /// <summary>�������� ��������������</summary>
        public double Parameter { get; set; }

        protected SimpleDoubleValueConverter(double Parameter, Conversion? to = null, Conversion? from = null)
            : this(to, from) => this.Parameter = Parameter;

        protected SimpleDoubleValueConverter([CanBeNull] Conversion? to = null, [CanBeNull] Conversion? from = null)
        {
            _To = to ?? To;
            _From = from ?? From;
        }

        /// <summary>������ �������������� ��������</summary>
        /// <param name="v">������������� ��������</param>
        /// <param name="p">��������� �������� ��������������. � ������ ���������� ������ �������� ��������� �������</param>
        /// <returns>��������������� ��������</returns>
        protected virtual double To(double v, double p) => throw new NotImplementedException("������ �������������� �� �����������");

        /// <summary>�������� �������������� ��������</summary>
        /// <param name="v">��������������� ��������</param>
        /// <param name="p">��������� �������� ��������������. � ������ ���������� ������ �������� ��������� �������</param>
        /// <returns>�������� ��������</returns>
        protected virtual double From(double v, double p) => throw new NotSupportedException("�������� �������������� �� ��������������");

        /// <inheritdoc />
        protected override double Convert(double v, double? p = null) => _To(v, p ?? Parameter);

        /// <inheritdoc />
        protected override double ConvertBack(double v, double? p = null) => _From(v, p ?? Parameter);
    }
}