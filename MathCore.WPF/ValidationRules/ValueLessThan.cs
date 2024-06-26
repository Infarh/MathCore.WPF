﻿using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

// ReSharper disable UnusedType.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.ValidationRules;

public class ValueLessThan : ValidationRule
{
    [ConstructorArgument(nameof(Value))]
    public double Value { get; set; }

    public bool IsEquals { get; set; }

    public string? ErrorMessage { get; set; }

    public ValueLessThan() { }

    public ValueLessThan(double value) => Value = value;

    /// <inheritdoc />
    public override ValidationResult Validate(object? value, CultureInfo CultureInfo)
    {
        if (value is null) return new(false, "Значение не указано");
        try
        {
            var v = Convert.ToDouble(value);
            return v < Value || IsEquals && v.Equals(Value)
                ? ValidationResult.ValidResult
                : new(false, ErrorMessage ?? $"Значение {value} больше чем {Value}");
        }
        catch (Exception e)
        {
            return new(false, $"Значение {value} не может быть преобразовано в вещественное число: {e.Message}");
        }
    }
}