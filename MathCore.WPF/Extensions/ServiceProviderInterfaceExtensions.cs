using System.Windows.Markup;
using System.Xaml;

// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System;

public static class ServiceProviderInterfaceExtensions
{
    public static IRootObjectProvider? GetRootObjectProvider(this IServiceProvider sp) => sp.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;

    public static IAmbientProvider? GetAmbientProvider(this IServiceProvider sp) => sp.GetService(typeof(IAmbientProvider)) as IAmbientProvider;

    public static IDestinationTypeProvider? GetDestinationTypeProvider(this IServiceProvider sp) => sp.GetService(typeof(IDestinationTypeProvider)) as IDestinationTypeProvider;

    public static IXamlNameProvider? GetXamlNameProvider(this IServiceProvider sp) => sp.GetService(typeof(IXamlNameProvider)) as IXamlNameProvider;

    public static IXamlSchemaContextProvider? GetXamlSchemaContextProvider(this IServiceProvider sp) => sp.GetService(typeof(IXamlSchemaContextProvider)) as IXamlSchemaContextProvider;

    public static IProvideValueTarget? GetValueTargetProvider(this IServiceProvider sp) => sp.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

}