using System;
using System.Threading;

namespace MathCore.WPF.Services
{
    public interface IProgressInfo : IDisposable
    {
        IProgress<double> Progress { get; }
        IProgress<string> Information { get; }
        IProgress<string> Status { get; }

        CancellationToken Cancel { get; }
    }
}