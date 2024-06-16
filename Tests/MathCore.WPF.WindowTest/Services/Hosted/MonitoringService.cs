using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OperationCanceledException = System.OperationCanceledException;

namespace MathCore.WPF.WindowTest.Services.Hosted;

public class MonitoringService(ILogger<MonitoringService> Logger) : 
    IHostedService,
    IDisposable
{
    private CancellationTokenSource _Cancellation;
    private Task _MonitoringTask;

    public Task StartAsync(CancellationToken cancellation)
    {
        Logger.LogInformation("Start monitoring");

        _Cancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellation);

        _MonitoringTask = MonitoringAsync(_Cancellation.Token);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken Cancel)
    {
        await _Cancellation.CancelAsync();

        try
        {
            await _MonitoringTask;
        }
        catch (OperationCanceledException)
        {
            Logger.LogInformation("Stop monitoring");

        }
    }

    private async Task MonitoringAsync(CancellationToken Cancel)
    {
        while (!Cancel.IsCancellationRequested)
        {
            Logger.LogInformation("Monitoring...");
            await Task.Delay(1000, Cancel).ConfigureAwait(false);
        }

        Cancel.ThrowIfCancellationRequested();
    }

    public void Dispose() => _Cancellation.Dispose();
}