using System;
using System.Windows.Markup;

namespace MathCore.WPF
{
    public class EnumValues : MarkupExtension
    {
        private Type _Type;
        public Type Type
        {
            get => _Type;
            set
            {
                if (value != null && !value.IsEnum) throw new ArgumentException("Тип не является перечислением", nameof(value));
                _Type = value;
            }
        }

        public EnumValues() { }

        public EnumValues(Type type)
        {
            if (!type.IsEnum) throw new ArgumentException("Тип не является перечислением", nameof(type));
            Type = type;
        }

        public override object ProvideValue(IServiceProvider sp) => _Type?.GetEnumValues();
    }
}