using System.Diagnostics;
using System.IO;
using System.IO.Packaging;

namespace MathCore.WPF.TeX;

internal sealed class MemoryPackage : IDisposable
{
    private static int __PackageCounter;

    private readonly Uri _PackageUri = new("payload://memorypackage" + Interlocked.Increment(ref __PackageCounter), UriKind.Absolute);
    private readonly Package _Package = Package.Open(new MemoryStream(), FileMode.Create);
    private int _PartCounter;

    public MemoryPackage() => PackageStore.AddPackage(this._PackageUri, this._Package);

    public Uri CreatePart(Stream stream, string ContentType = "application/octet-stream")
    {
        var part_uri = new Uri("/stream" + ++_PartCounter, UriKind.Relative);

        var part = _Package.CreatePart(part_uri, ContentType);

        Debug.Assert(part != null, "part != null");
        using(var part_stream = part.GetStream())
            CopyStream(stream, part_stream);

        // Each packUri must be globally unique because WPF might perform some caching based on it.
        return PackUriHelper.Create(this._PackageUri, part_uri);
    }

    public void DeletePart(Uri PackUri) => _Package.DeletePart(PackUriHelper.GetPartUri(PackUri));

    public void Dispose()
    {
        PackageStore.RemovePackage(_PackageUri);
        _Package.Close();
    }

    private static void CopyStream(Stream source, Stream destination)
    {
        const int buffer_size = 4096;

        var buffer = new byte[buffer_size];
        int read;
        while((read = source.Read(buffer, 0, buffer.Length)) != 0)
            destination.Write(buffer, 0, read);
    }
}