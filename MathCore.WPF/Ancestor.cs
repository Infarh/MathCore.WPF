using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF;

[MarkupExtensionReturnType(typeof(RelativeSource))]
public class Ancestor(Type AncestorType) : MarkupExtension, ISupportInitialize
{
    public Ancestor() : this(null) { }

    public Ancestor(Type AncestorType, int AncestorLevel) : this(AncestorType) => this.AncestorLevel = Math.Max(1, AncestorLevel);


    [ConstructorArgument(nameof(AncestorType))]
    public Type AncestorType { get; set; } = AncestorType;

    [ConstructorArgument(nameof(AncestorLevel))]
    public int AncestorLevel { get; set; } = 1;

   public override object ProvideValue(IServiceProvider sp) => new RelativeSource(RelativeSourceMode.FindAncestor, AncestorType, AncestorLevel);

    void ISupportInitialize.BeginInit() { }

    void ISupportInitialize.EndInit()
    {
        if (AncestorType is null)
            throw new InvalidOperationException("Тип не задан");
    }
}