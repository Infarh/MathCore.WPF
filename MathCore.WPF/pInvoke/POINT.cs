using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MathCore.WPF.pInvoke;

[StructLayout(LayoutKind.Sequential), SuppressMessage("ReSharper", "InconsistentNaming")]
internal struct Point : IEquatable<Point>
{
    /// <summary>x coordinate of point</summary>
    public int x;
    /// <summary>y coordinate of point</summary>
    public int y;

    /// <summary>Construct a point of coordinates (x,y)</summary>
    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public bool Equals(Point other) => x == other.x && y == other.y;

    public override bool Equals(object? obj) => obj is Point other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(x, y);

    public static bool operator ==(Point left, Point right) => left.Equals(right);

    public static bool operator !=(Point left, Point right) => !left.Equals(right);
}