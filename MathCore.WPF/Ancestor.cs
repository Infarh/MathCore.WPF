using System;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF
{
    [MarkupExtensionReturnType(typeof(RelativeSource))]
    public class Ancestor : MarkupExtension, ISupportInitialize
    {
        [ConstructorArgument(nameof(AncestorType))]
        public Type AncestorType { get; set; }

        [ConstructorArgument(nameof(AncestorLevel))]
        public int AncestorLevel { get; set; } = 1;

        public Ancestor() { }

        public Ancestor(Type AncestorType) => this.AncestorType = AncestorType;

        public Ancestor(Type AncestorType, int AncestorLevel) : this(AncestorType) => this.AncestorLevel = Math.Max(1, AncestorLevel);

        public override object ProvideValue(IServiceProvider sp) => new RelativeSource(RelativeSourceMode.FindAncestor, AncestorType, AncestorLevel);

        void ISupportInitialize.BeginInit() { }

        void ISupportInitialize.EndInit()
        {
            if (AncestorType is null)
                throw new InvalidOperationException("Тип не задан");
        }
    }
}
