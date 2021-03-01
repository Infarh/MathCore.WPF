using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Animation;
// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF
{
    [MarkupExtensionReturnType(typeof(Binding))]
    public class AnimatedBinding : MarkupExtension
    {
        private static readonly DependencyPropertyDescriptor __DoubleAnimationToPropertyDescriptor =
            DependencyPropertyDescriptor.FromProperty(DoubleAnimation.ToProperty, typeof(DoubleAnimation));

        [ConstructorArgument("path")]
        public PropertyPath Path { get; set; }
        public string XPath { get; set; }
        public string ElementName { get; set; }
        public RelativeSource RelativeSource { get; set; }
        public object Source { get; set; }
        public BindingMode Mode { get; set; } = BindingMode.Default;
        public object TargetNullValue { get; set; }
        public IValueConverter Converter { get; set; }
        public object ConverterParameter { get; set; }
        public CultureInfo ConverterCulture { get; set; }
        public bool ValidatesOnExceptions { get; set; }
        public bool ValidatesOnDataErrors { get; set; }
        public Duration Duration { get; set; } = new(TimeSpan.FromSeconds(0.1));
        public IEasingFunction EasingFunction { get; set; }
        public double SpeedRatio { get; set; } = 1;
        public double AccelerationRatio { get; set; } = 0.2;
        public double DecelerationRatio { get; set; } = 0.8;
        public bool NotifyOnTargetUpdated { get; set; }
        public bool NotifyOnSourceUpdated { get; set; }
        public UpdateSourceTrigger UpdateSourceTrigger { get; set; } = UpdateSourceTrigger.Default;
        public Collection<ValidationRule> ValidationRules { get; } = new();
        public bool IsAsync { get; set; }
        public object AsyncState { get; set; }
        public bool NotifyOnValidationError { get; set; }
        public object FallbackValue { get; set; } = DependencyProperty.UnsetValue;
        public string StringFormat { get; set; }
        public string BindingGroupName { get; set; } = "";
        public bool BindsDirectlyToSource { get; set; }
        public UpdateSourceExceptionFilterCallback UpdateSourceExceptionFilter { get; set; }
        public AnimatedBinding(PropertyPath path) => Path = path;

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider service)
        {
            if (service.GetService(typeof(IProvideValueTarget)) is not IProvideValueTarget value_provider)
                throw new InvalidOperationException("Невозможно получить источник данных о цели привязки");

            if (value_provider.TargetObject is not FrameworkElement target) throw new InvalidOperationException("Не определны сведенья о целевом объекте привязки");
            if (value_provider.TargetProperty is not DependencyProperty property) throw new InvalidOperationException("Не определны сведенья о целевом свойстве привязки");

            var binding = new Binding
            {
                Path = Path,
                Converter = Converter,
                ConverterParameter = ConverterParameter,
                ConverterCulture = ConverterCulture,
                ValidatesOnExceptions = ValidatesOnExceptions,
                ValidatesOnDataErrors = ValidatesOnDataErrors,
                TargetNullValue = TargetNullValue,
                UpdateSourceTrigger = UpdateSourceTrigger,
                NotifyOnSourceUpdated = NotifyOnSourceUpdated,
                NotifyOnTargetUpdated = NotifyOnTargetUpdated,
                NotifyOnValidationError = NotifyOnValidationError,
                Mode = Mode,
                IsAsync = IsAsync,
                AsyncState = AsyncState,
                BindsDirectlyToSource = BindsDirectlyToSource,
                BindingGroupName = BindingGroupName,
                StringFormat = StringFormat,
                FallbackValue = FallbackValue,
                UpdateSourceExceptionFilter = UpdateSourceExceptionFilter
            };

            if (!string.IsNullOrWhiteSpace(ElementName))
                binding.ElementName = ElementName;
            else if (RelativeSource != null)
                binding.RelativeSource = RelativeSource;
            else if (Source != null)
                binding.Source = Source;

            if (ValidationRules.Count > 0)
            {
                var rules = binding.ValidationRules;
                foreach (var rule in ValidationRules) rules.Add(rule);
            }

            if (!string.IsNullOrWhiteSpace(XPath)) binding.XPath = XPath;

            var animation = new DoubleAnimation
            {
                Duration = Duration,
                AccelerationRatio = AccelerationRatio,
                DecelerationRatio = DecelerationRatio,
                EasingFunction = EasingFunction,
                SpeedRatio = SpeedRatio
            };
            if (EasingFunction != null) animation.EasingFunction = EasingFunction;

            __DoubleAnimationToPropertyDescriptor.AddValueChanged(animation, (_, _) => target.BeginAnimation(property, animation));
            BindingOperations.SetBinding(animation, DoubleAnimation.ToProperty, binding);

            return DependencyProperty.UnsetValue;
        }
    }
}
