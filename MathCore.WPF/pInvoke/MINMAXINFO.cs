using System.Runtime.InteropServices;
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.pInvoke;

[StructLayout(LayoutKind.Sequential)]
internal struct MinMaxInfo : IEquatable<MinMaxInfo>
{
    public Point Reserved;
    public Point MaxSize;
    public Point MaxPosition;
    public Point MinTrackSize;
    public Point MaxTrackSize;

    public bool Equals(MinMaxInfo other) => Reserved.Equals(other.Reserved) && MaxSize.Equals(other.MaxSize) && MaxPosition.Equals(other.MaxPosition) && MinTrackSize.Equals(other.MinTrackSize) && MaxTrackSize.Equals(other.MaxTrackSize);

    public override bool Equals(object? obj) => obj is MinMaxInfo other && Equals(other);

    public override int GetHashCode() => HashBuilder.New(Reserved)
       .Append(MaxSize)
       .Append(MaxPosition)
       .Append(MinTrackSize)
       .Append(MaxTrackSize);

    public static bool operator ==(MinMaxInfo left, MinMaxInfo right) => left.Equals(right);

    public static bool operator !=(MinMaxInfo left, MinMaxInfo right) => !left.Equals(right);
}