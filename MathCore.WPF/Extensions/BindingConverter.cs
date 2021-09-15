using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Extensions
{
    public class BindingConverter : ExpressionConverter
    {
        /// <inheritdoc />
        public override bool CanConvertTo(ITypeDescriptorContext c, Type dt) => dt == typeof(MarkupExtension);

        /// <inheritdoc />
        public override object? ConvertTo(ITypeDescriptorContext context, CultureInfo c, object v, Type t) =>
            t == typeof(MarkupExtension)
                ? v is not BindingExpression binding
                    ? throw new ApplicationException()
                    : binding.ParentBinding
                : base.ConvertTo(context, c, v, t);
    }

    public static class TypeConvertersRegistrator
    {
        public static void Register<T, TConverter>() => typeof(T).AddConverter(typeof(TConverter));

        public static void RegisterBindingConverter()
        {
            var binding_type = typeof(Binding);
            binding_type.AddConverter(typeof(BindingConverter));
            binding_type.AddProvider(new BindingTypeDescriptionProvider());
        }
    }

    public class BindingTypeDescriptionProvider : TypeDescriptionProvider
    {
        private static readonly TypeDescriptionProvider __DefaultTypeProvider = typeof(Binding).GetProvider();

        public BindingTypeDescriptionProvider() : base(__DefaultTypeProvider) { }

        /// <inheritdoc />
        public override ICustomTypeDescriptor GetTypeDescriptor(Type ObjectType, object? Instance) => 
            Instance is null 
                ? base.GetTypeDescriptor(ObjectType, null)! 
                : new BindingCustomTypeDescriptor(base.GetTypeDescriptor(ObjectType, Instance)!);
    }

    public class BindingCustomTypeDescriptor : CustomTypeDescriptor
    {
        public BindingCustomTypeDescriptor(ICustomTypeDescriptor parent) : base(parent) { }

        /// <inheritdoc />
        public override PropertyDescriptorCollection GetProperties() => GetProperties(Array.Empty<Attribute>());

        private static readonly string[] __Properties = { "Source", "ValidationRules" };

        /// <inheritdoc />
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var descriptors = new PropertyDescriptorCollection(base.GetProperties().Cast<PropertyDescriptor>().ToArray());
            foreach (var property_descriptor in __Properties.Select(p => descriptors.Find(p, false)))
            {
                var new_property_descriptor = TypeDescriptor.CreateProperty(
                    typeof(Binding),
                    property_descriptor,
                    new DefaultValueAttribute(null),
                    new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content));
                descriptors.Remove(property_descriptor);
                descriptors.Add(new_property_descriptor);
            }
            return descriptors;
        }
    }
}