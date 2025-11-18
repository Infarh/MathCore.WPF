#if !NET5_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Diagnostics.CodeAnalysis;

/// <summary>Specifies that <see langword="null" /> is allowed as an input even if the corresponding type disallows it.</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false)]
public sealed class AllowNullAttribute : Attribute
{
}

/// <summary>Specifies that an output may be <see langword="null" /> even if the corresponding type disallows it.</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
public sealed class MaybeNullAttribute : Attribute
{
}

#endif