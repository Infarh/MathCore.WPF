using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MathCore.WPF.pInvoke
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto), SuppressMessage("ReSharper", "InconsistentNaming")]
    public class MONITORINFO
    {
        /// <summary>
        /// </summary>            
        public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

        /// <summary>
        /// </summary>            
        public RECT rcMonitor = new RECT();

        /// <summary>
        /// </summary>            
        public RECT rcWork = new RECT();

        /// <summary>
        /// </summary>            
        public int dwFlags = 0;
    }
}