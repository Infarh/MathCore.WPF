using System;
using System.Windows.Markup;
using System.Xaml;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Extensions
{
    public static class ServiceProviderInterfaceExtensions
    {
        [CanBeNull] public static IRootObjectProvider? GetRootObjectProvider([NotNull] this IServiceProvider sp) => sp.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;

        [CanBeNull] public static IAmbientProvider? GetAmbientProvider([NotNull] this IServiceProvider sp) => sp.GetService(typeof(IAmbientProvider)) as IAmbientProvider;

        [CanBeNull] public static IDestinationTypeProvider? GetDestinationTypeProvider([NotNull] this IServiceProvider sp) => sp.GetService(typeof(IDestinationTypeProvider)) as IDestinationTypeProvider;

        [CanBeNull] public static IXamlNameProvider? GetXamlNameProvider([NotNull] this IServiceProvider sp) => sp.GetService(typeof(IXamlNameProvider)) as IXamlNameProvider;

        [CanBeNull] public static IXamlSchemaContextProvider? GetXamlSchemaContextProvider([NotNull] this IServiceProvider sp) => sp.GetService(typeof(IXamlSchemaContextProvider)) as IXamlSchemaContextProvider;

        [CanBeNull] public static IProvideValueTarget? GetValueTargetProvider([NotNull] this IServiceProvider sp) => sp.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

    }
}