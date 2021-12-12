#if !NET5_0_OR_GREATER
using System.ComponentModel;

// ReSharper disable CheckNamespace

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}
#endif