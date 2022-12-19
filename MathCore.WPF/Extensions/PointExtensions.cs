// ReSharper disable once CheckNamespace
namespace System.Windows;

public static class PointExtensions
{
    public static void Deconstruct(this Point point, out double X, out double Y) => (X, Y) = (point.X, point.Y);

    public static Point Add(this Point point, double x, double y) => new(point.X + x, point.Y + y);

    public static Point Substrate(this Point point, double x, double y) => new(point.X - x, point.Y - y);

    public static Point Add(this Point a, Point b) => new(a.X + b.X, a.Y + b.Y);

    public static Point Substrate(this Point a, Point b) => new(a.X - b.X, a.Y - b.Y);

    public static Point Mul(this Point a, double k) => new(a.X * k, a.Y * k);

    public static Point Mul(this Point a, double kx, double ky) => new(a.X * kx, a.Y * ky);

    public static Point Div(this Point a, double k) => new(a.X / k, a.Y / k);

    public static Point Div(this Point a, double kx, double ky) => new(a.X / kx, a.Y / ky);
}
