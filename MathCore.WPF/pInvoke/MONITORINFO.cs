using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.pInvoke
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto), SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class MonitorInfo
    {
        public int Size = Marshal.SizeOf(typeof(MonitorInfo));

        public Rect Monitor = new Rect();

        public Rect Work = new Rect();

        public int Flags = 0;
    }
}