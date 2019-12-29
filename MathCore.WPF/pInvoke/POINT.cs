using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MathCore.WPF.pInvoke
{
    [StructLayout(LayoutKind.Sequential), SuppressMessage("ReSharper", "InconsistentNaming")]
    public struct POINT
    {
        /// <summary>x coordinate of point</summary>
        public int x;
        /// <summary>y coordinate of point</summary>
        public int y;

        /// <summary>Construct a point of coordinates (x,y)</summary>
        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}