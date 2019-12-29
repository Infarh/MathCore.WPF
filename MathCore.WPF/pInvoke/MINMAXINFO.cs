using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MathCore.WPF.pInvoke
{
    [StructLayout(LayoutKind.Sequential), SuppressMessage("ReSharper", "InconsistentNaming")]
    public struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }
}