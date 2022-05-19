using System.Globalization;
using System.Windows.Data;
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Extensions;

[Copyright("http://habrahabr.ru/users/Makeman/", url = "http://habrahabr.ru/post/254115/")]
public abstract class BindingExtension : Binding, IValueConverter
{
    protected BindingExtension() => Source = Converter = this;

    protected BindingExtension(object Source)
    {
        this.Source = Source;
        Converter = this;
    }

    protected BindingExtension(RelativeSource RelativeSource)
    {
        this.RelativeSource = RelativeSource;
        Converter = this;
    }

    public abstract object? Convert(object? v, Type? t, object? p, CultureInfo? c);

    public virtual object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) => 
        throw new NotSupportedException();
}